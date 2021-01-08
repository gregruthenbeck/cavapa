using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;

namespace cavapa
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
            Console.WriteLine("Running in {0}-bit mode.", Environment.Is64BitProcess ? "64" : "32");

            FFmpegBinariesHelper.RegisterFFmpegBinaries();

            Console.WriteLine($"FFmpeg version info: {ffmpeg.av_version_info()}");

            SetupLogging();

            Task.Run(() =>
            {
                Console.WriteLine("Task={0}, Thread={1}", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
                DecodeAllFramesToImages(AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2);
            });

            //ConfigureHWDecoder(out var deviceType);
            //var deviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2;

            //Console.WriteLine("Decoding...");
            //DecodeAllFramesToImages(deviceType);

            //Console.WriteLine("Encoding...");
            //EncodeImagesToH264();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }

        private void SetImage(Image img) {
            pictureBox1.Image = img;
        }

        private static void ConfigureHWDecoder(out AVHWDeviceType HWtype)
        {
            HWtype = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
            Console.WriteLine("Use hardware acceleration for decoding?[n]");
            var key = Console.ReadLine();
            var availableHWDecoders = new Dictionary<int, AVHWDeviceType>();
            if (key == "y")
            {
                Console.WriteLine("Select hardware decoder:");
                var type = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
                var number = 0;
                while ((type = ffmpeg.av_hwdevice_iterate_types(type)) != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
                {
                    Console.WriteLine($"{++number}. {type}");
                    availableHWDecoders.Add(number, type);
                }
                if (availableHWDecoders.Count == 0)
                {
                    Console.WriteLine("Your system have no hardware decoders.");
                    HWtype = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
                    return;
                }
                int decoderNumber = availableHWDecoders.SingleOrDefault(t => t.Value == AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2).Key;
                if (decoderNumber == 0)
                    decoderNumber = availableHWDecoders.First().Key;
                Console.WriteLine($"Selected [{decoderNumber}]");
                int.TryParse(Console.ReadLine(), out var inputDecoderNumber);
                availableHWDecoders.TryGetValue(inputDecoderNumber == 0 ? decoderNumber : inputDecoderNumber, out HWtype);
            }
        }

        private static unsafe void SetupLogging()
        {
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);

            // do not convert to local function
            av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
            {
                if (level > ffmpeg.av_log_get_level()) return;

                var lineSize = 1024;
                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;
                ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(line);
                Console.ResetColor();
            };

            ffmpeg.av_log_set_callback(logCallback);
        }

        private unsafe void DecodeAllFramesToImages(AVHWDeviceType HWDevice)
        {
            // decode all frames from url, please note that it can be a local or remote resource, e.g. string url = "../../sample_mpeg4.mp4";
            var url = "CameraB_1min.mp4";

            // Search for the sample file by popping dirs until we either find it or run out of pops
            var current = Environment.CurrentDirectory;
            while (current != null)
            {
                var path = Path.Combine(current, url);
                if (File.Exists(path))
                {
                    Console.WriteLine($"Sample video found in: {path}");
                    url = path;
                    break;
                }

                current = Directory.GetParent(current)?.FullName;
            }

            using (var vsd = new VideoStreamDecoder(url, HWDevice))
            {
                Console.WriteLine($"codec name: {vsd.CodecName}");

                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = HWDevice == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE ? vsd.PixelFormat : GetHWPixelFormat(HWDevice);
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                {
                    var frameNumber = 0;
                    //byte[] currFrameData = new byte[destinationSize.Width * destinationSize.Height * 3];
                    //byte[] prevFrameData = new byte[destinationSize.Width * destinationSize.Height * 3];
                    var width = destinationSize.Width;
                    var height = destinationSize.Height;

                    Image<Bgr, byte> prevImage = new Image<Bgr, byte>(width, height); //Image Class from Emgu.CV
                    int frameBlendInterval = 25;
                    FrameBlender backgroundBuilder = new FrameBlender(width, height, 10);
                    var background = new Image<Bgr, byte>(width, height);
                    var currForeground = new Image<Bgr, byte>(width, height);

                    while (vsd.TryDecodeNextFrame(out var frame))
                    {
                        var convertedFrame = vfc.Convert(frame);

                        Image<Bgr, byte> currImage = new Image<Bgr, byte>(width, height, convertedFrame.linesize[0], (IntPtr)convertedFrame.data[0]);

                        if (frameNumber % frameBlendInterval == 0)
                            background = backgroundBuilder.Update(currImage.Mat).ToImage<Bgr,byte>(); //.Save($"bg{frameNumber}.jpg", ImageFormat.Jpeg);

                        Mat foregroundMat = background.Not().Mat + currImage.Mat;
                        currForeground = foregroundMat.ToImage<Bgr,byte>();

                        MethodInvoker m = new MethodInvoker(() => pictureBox1.Image = currForeground.ToBitmap());
                        pictureBox1.Invoke(m);

                        //Mat diff = currImage.Not().Mat + prevImage.Mat;
                        //Image<Bgr, byte> deltaImage = diff.ToImage<Bgr, byte>().Not();
                        //MethodInvoker m = new MethodInvoker(() => pictureBox1.Image = deltaImage.ToBitmap());
                        //pictureBox1.Invoke(m);

                        prevImage.Bytes = currImage.Bytes;

                        Console.WriteLine($"frame: {frameNumber}");
                        frameNumber++;
                    }
                }
            }
        }

        Bitmap ProcessOpenCV(byte[] curr, byte[] prev, int width, int height, int bpp = 3)
        {
            Image<Bgr, byte> currImage = new Image<Bgr, byte>(width, height); //Image Class from Emgu.CV
            Image<Bgr, byte> prevImage = new Image<Bgr, byte>(width, height); //Image Class from Emgu.CV
            currImage.Bytes = curr;
            prevImage.Bytes = prev;
            Mat diff = currImage.Mat - prevImage.Mat;
            Image<Bgr, byte> deltaImage = diff.ToImage<Bgr, byte>();

            Rectangle fullRect = new Rectangle(0, 0, width, height);
            Bitmap ret = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData retData = ret.LockBits(fullRect, ImageLockMode.WriteOnly, ret.PixelFormat);
            unsafe
            {
                byte* src = (byte*)diff.DataPointer;
                for (int yi = 0; yi < ret.Height; yi++)
                {
                    byte* dst = (byte*)retData.Scan0.ToPointer() + yi * ret.Width * 3; // BGR24
                    for (int xi = 0; xi < ret.Width * 3; xi++, ++src, ++dst)
                    {
                        *dst = *src;
                    }
                }
            }
            ret.UnlockBits(retData);
            return ret;
        }


        unsafe Bitmap Process(byte* curr, byte* prev, int width, int height, int bpp = 3)
        {
            Rectangle fullRect = new Rectangle(0, 0, width, height);
            Bitmap ret = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData retData = ret.LockBits(fullRect, ImageLockMode.WriteOnly, ret.PixelFormat);

            unsafe
            {
                for (int yi = 0; yi < ret.Height; yi++)
                {
                    byte* res = (byte*)retData.Scan0.ToPointer() + yi * ret.Width * 3; // BGR24
                    byte* cur = curr + yi * ret.Width * bpp; // BGR24
                    byte* pre = prev + yi * ret.Width * bpp; // BGR24
                    for (int xi = 0; xi < ret.Width; xi++, res += 3, cur += 3, pre += 3)
                    {
                        *res = (byte)Math.Abs((int)*cur - (int)*pre);
                        *(res + 1) = (byte)Math.Abs((int)*(cur + 1) - (int)*(pre + 1));
                        *(res + 2) = (byte)Math.Abs((int)*(cur + 2) - (int)*(pre + 2));
                    }
                }
            }

            ret.UnlockBits(retData);

            return ret;
        }

        Bitmap ProcessGraphics(Bitmap curr, Bitmap prev) 
        {
            Rectangle fullRect = new Rectangle(0, 0, curr.Width, curr.Height);
            Bitmap ret = new Bitmap(curr.Width, curr.Height, curr.PixelFormat);
            using (Graphics g = Graphics.FromImage(ret))
            {
                g.DrawImageUnscaled(curr, 0, 0);
                g.DrawImageUnscaled(prev, 0, 0);
            }
            return ret;
        }

        Bitmap Process(Bitmap curr, Bitmap prev)
        {
            Rectangle fullRect = new Rectangle(0, 0, curr.Width, curr.Height);
            Bitmap ret = new Bitmap(curr.Width, curr.Height, curr.PixelFormat);

            BitmapData retData = ret.LockBits(fullRect, ImageLockMode.WriteOnly, ret.PixelFormat);
            BitmapData currData = curr.LockBits(fullRect, ImageLockMode.ReadOnly, ret.PixelFormat);
            BitmapData prevData = prev.LockBits(fullRect, ImageLockMode.ReadOnly, ret.PixelFormat);

            unsafe
            {
                for (int yi = 0; yi < curr.Height; yi++)
                {
                    byte* res = (byte*)retData.Scan0.ToPointer() + yi * curr.Width * 3; // BGR24
                    byte* cur = (byte*)currData.Scan0.ToPointer() + yi * curr.Width * 3; // BGR24
                    byte* pre = (byte*)prevData.Scan0.ToPointer() + yi * curr.Width * 3; // BGR24
                    for (int xi = 0; xi < curr.Width; xi++, res += 3, cur += 3, pre += 3)
                    {
                        *res = (byte)Math.Abs((int)*cur - (int)*pre);
                        *(res + 1) = (byte)Math.Abs((int)*(cur + 1) - (int)*(pre + 1));
                        *(res + 2) = (byte)Math.Abs((int)*(cur + 2) - (int)*(pre + 2));
                    }
                }
            }

            ret.UnlockBits(retData);
            curr.UnlockBits(currData);
            prev.UnlockBits(prevData);

            return ret;
        }

        private static AVPixelFormat GetHWPixelFormat(AVHWDeviceType hWDevice)
        {
            switch (hWDevice)
            {
                case AVHWDeviceType.AV_HWDEVICE_TYPE_NONE:
                    return AVPixelFormat.AV_PIX_FMT_NONE;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VDPAU:
                    return AVPixelFormat.AV_PIX_FMT_VDPAU;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA:
                    return AVPixelFormat.AV_PIX_FMT_CUDA;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VAAPI:
                    return AVPixelFormat.AV_PIX_FMT_VAAPI;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2:
                    return AVPixelFormat.AV_PIX_FMT_NV12;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_QSV:
                    return AVPixelFormat.AV_PIX_FMT_QSV;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VIDEOTOOLBOX:
                    return AVPixelFormat.AV_PIX_FMT_VIDEOTOOLBOX;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA:
                    return AVPixelFormat.AV_PIX_FMT_NV12;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_DRM:
                    return AVPixelFormat.AV_PIX_FMT_DRM_PRIME;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_OPENCL:
                    return AVPixelFormat.AV_PIX_FMT_OPENCL;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_MEDIACODEC:
                    return AVPixelFormat.AV_PIX_FMT_MEDIACODEC;
                default:
                    return AVPixelFormat.AV_PIX_FMT_NONE;
            }
        }

        private static unsafe void EncodeImagesToH264()
        {
            var frameFiles = Directory.GetFiles(".", "frame.*.jpg").OrderBy(x => x).ToArray();
            var fistFrameImage = Image.FromFile(frameFiles.First());

            var outputFileName = "out-h264.mp4";
            var fps = 25;
            var sourceSize = fistFrameImage.Size;
            var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
            var destinationSize = sourceSize;
            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
            {
                using (var fs = File.Open(outputFileName, FileMode.Create)) // be advise only ffmpeg based player (like ffplay or vlc) can play this file, for the others you need to go through muxing
                {
                    using (var vse = new H264VideoStreamEncoder(fs, fps, destinationSize))
                    {
                        var frameNumber = 0;
                        foreach (var frameFile in frameFiles)
                        {
                            byte[] bitmapData;

                            using (var frameImage = Image.FromFile(frameFile))
                            using (var frameBitmap = frameImage is Bitmap bitmap ? bitmap : new Bitmap(frameImage))
                            {
                                bitmapData = GetBitmapData(frameBitmap);
                            }

                            fixed (byte* pBitmapData = bitmapData)
                            {
                                var data = new byte_ptrArray8 { [0] = pBitmapData };
                                var linesize = new int_array8 { [0] = bitmapData.Length / sourceSize.Height };
                                var frame = new AVFrame
                                {
                                    data = data,
                                    linesize = linesize,
                                    height = sourceSize.Height
                                };
                                var convertedFrame = vfc.Convert(frame);
                                convertedFrame.pts = frameNumber * fps;
                                vse.Encode(convertedFrame);
                            }

                            Console.WriteLine($"frame: {frameNumber}");
                            frameNumber++;
                        }
                    }
                }
            }
        }

        private static byte[] GetBitmapData(Bitmap frameBitmap)
        {
            var bitmapData = frameBitmap.LockBits(new Rectangle(Point.Empty, frameBitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                var length = bitmapData.Stride * bitmapData.Height;
                var data = new byte[length];
                Marshal.Copy(bitmapData.Scan0, data, 0, length);
                return data;
            }
            finally
            {
                frameBitmap.UnlockBits(bitmapData);
            }
        }
    }
}
