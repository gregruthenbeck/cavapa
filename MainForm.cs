﻿using System;
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
using System.Diagnostics;
using System.Reflection;

namespace cavapa
{
    public partial class MainForm : Form
    {
        Version version;
        string versionInfo;
        string videoFilepath = "";
        string csvExportPath = "";

        bool maskSet = true;
        bool processingEnabled = true;
        bool processingSleep = false;
        ProcessSettings processSettings = new ProcessSettings();
        long videoFrameCount = 0;
        float videoFrameRate = 25.0f;
        float[] movementScores = null;
        float movementScoreMax = 0;
        bool processMultithreaded = false;
        long processedFrameCount = 0L;
        long frameNumber = 0L;

        Image<Gray, byte> movement;
        Image<Gray, byte> movementHist;

        FPSTimer perfTimer = null;
        Bitmap bmpChart = null;
        PictureBox pictureBoxChart = null;

        private static int _trackBarPos = 0;

        public MainForm()
        {
            InitializeComponent();

            var execAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            version = execAssembly.GetName().Version;
            versionInfo = GetInformationalVersion(execAssembly);
            this.Text += " v" + version.ToString() + "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
            Console.WriteLine("Running in {0}-bit mode.", Environment.Is64BitProcess ? "64" : "32");
            Console.WriteLine("Number of processors: {0}", Environment.ProcessorCount);
            FFmpegBinariesHelper.RegisterFFmpegBinaries();
            Console.WriteLine($"FFmpeg version info: {ffmpeg.av_version_info()}");
            SetupLogging();

            processSettings.movementHistoryDecay = 0.85;
            //processSettings.frameBlendCount = 2;
            //processSettings.movementMultiplier = 20.0;

            // Test BGBuilder
            //foreach (var f in Directory.GetFiles(".", "bg*.jpg"))
            //    File.Delete(f);
            //var bgb = new BGBuilder(AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2, "../../../CameraB_cut.mp4");
            //var bgb = new BGBuilder(AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2, "../../../kilp_2011_8_22-10-koris-deint.mp4");

            enableFlickerReductionToolStripMenuItem.Checked = (processSettings.frameBlendCount > 1);
            enableShadowReductionToolStripMenuItem.Checked = processSettings.enableShadowReduction;

            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            pictureBoxChart = new PictureBox();
            bmpChart = new Bitmap(this.tableLayoutPanel1.Size.Width, 120, PixelFormat.Format32bppArgb);
            pictureBoxChart.Size = new Size(bmpChart.Width, bmpChart.Height);
            pictureBoxChart.Margin = new Padding(0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxChart, 0, 2);
        }

        public string GetInformationalVersion(Assembly assembly)
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }

        private void UpdateChart()
        {
            int interval = Math.Max((int)(videoFrameCount / (2L * (long)tableLayoutPanel1.Width)), 1); // Max(,1) to handle short videos

            if (frameNumber % interval != 0 ||
                frameNumber / interval < 2)
                return;

            PointF[] points = new PointF[videoFrameCount / interval];
            SizeF s = new SizeF(.5f, (float)bmpChart.Height / movementScoreMax);
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PointF((float)i * s.Width, bmpChart.Height - movementScores[i * interval] * s.Height);
                points[i].Y = Math.Min(points[i].Y, (float)bmpChart.Height-2.0f);
            }
            points[0] = new PointF(0F, (float)(bmpChart.Height - 1));

            using (Graphics g = Graphics.FromImage(bmpChart))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.FillRectangle(Brushes.LightGray, 0, 0, bmpChart.Width, bmpChart.Height);
                g.DrawLines(Pens.Blue, points);
            }

            MethodInvoker m = new MethodInvoker(() => pictureBoxChart.Image = bmpChart);
            pictureBoxChart.Invoke(m);
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

        private unsafe void ProcessFrames(AVHWDeviceType HWDevice, string url, int chunkId = 0, long startFrame = 0, long endFrame = long.MaxValue)
        {
            videoFilepath = url;
            processingEnabled = true;
            perfTimer = new FPSTimer((int)videoFrameRate);
            long leadInFrames = processSettings.backgroundFrameBlendCount * processSettings.backgroundFrameBlendInterval;
            if (!processMultithreaded && leadInFrames < startFrame) // dont't do leadIn if we're too close to the start of the video
                startFrame = startFrame - leadInFrames;
            else
                leadInFrames = 0L;

            using (var vsd = new VideoStreamDecoder(url, HWDevice))
            {
                Console.WriteLine($"codec name: {vsd.CodecName}");

                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = HWDevice == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE ? vsd.PixelFormat : GetHWPixelFormat(HWDevice);
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                int framesSinceSeek = 0;
                int framesSinceSeekThresh = 5;

                using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                {
                    frameNumber = startFrame;
                    if (startFrame != 0)
                        vsd.Seek(startFrame);
                    //byte[] currFrameData = new byte[destinationSize.Width * destinationSize.Height * 3];
                    //byte[] prevFrameData = new byte[destinationSize.Width * destinationSize.Height * 3];
                    var width = destinationSize.Width;
                    var height = destinationSize.Height;

                    Image<Bgr, byte> prevImage = new Image<Bgr, byte>(width, height); //Image Class from Emgu.CV
                    //FrameBlender[] backgroundBuilders = new FrameBlender[processSettings.backgroundFrameBlendInterval];
                    //Bitmap[] bgs = new Bitmap[processSettings.backgroundFrameBlendInterval];
                    //for (int i = 0; i < backgroundBuilders.Length; i++) {
                    //    backgroundBuilders[i] = new FrameBlender(width, height, processSettings.backgroundFrameBlendCount);
                    //    bgs[i] = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    //}
                    FrameBlender frameSmoother = new FrameBlender(width, height, processSettings.frameBlendCount);
                    Image<Bgra, byte> background = null;
                    var bgBuilder = new Emgu.CV.BackgroundSubtractorMOG2(250, 16, false);
                    Image<Gray, byte> foregroundMask = new Image<Gray, byte>(width, height);
                    var currForeground = new Image<Bgr, byte>(width, height);
                    var prevForeground = new Image<Bgr, byte>(width, height);
                    //var movement = new Image<Gray, byte>(width, height);
                    //var movementHist = new Image<Gray, byte>(width, height);
                    Image<Gray, byte> mask = null;

                    bool decoderActive = true;
                    AVFrame frame;
                    while (decoderActive && (frameNumber < endFrame) && processingEnabled)
                    {
                        int seekFrameNum = _trackBarPos;
                        if (!processMultithreaded)
                        {
                            if (Math.Abs(frameNumber - seekFrameNum) > 250)
                            {
                                vsd.Seek(seekFrameNum);
                                frameNumber = seekFrameNum;
                                framesSinceSeek = 0;
                            }

                            var trackBarSetMI = new MethodInvoker(() => trackBar1.Value = Math.Min((int)frameNumber, trackBar1.Maximum - 1));
                            trackBar1.Invoke(trackBarSetMI);
                        }

                        try
                        {
                            decoderActive = vsd.TryDecodeNextFrame(out frame);
                        }
                        catch
                        {
                            continue;
                        }

                        if (framesSinceSeek < framesSinceSeekThresh)
                            ++framesSinceSeek;

                        while (processingSleep)
                            Thread.Sleep(500);

                        var convertedFrame = vfc.Convert(frame);

                        Image<Bgr, byte> currImage = new Image<Bgr, byte>(width, height, convertedFrame.linesize[0], (IntPtr)convertedFrame.data[0]);
                        // Shadow reduction: Shadows are lindear and less vertical, so stretch the image wider
                        // and then resize back to original aspect-ratio to discard some horizontal detail.
                        // Also, people are taller than bikes & balls
                        if (processSettings.enableShadowReduction)
                            currImage = currImage.Resize(width, height / 8, Emgu.CV.CvEnum.Inter.Area).Resize(width, height, Emgu.CV.CvEnum.Inter.Area);

                        //if (processSettings.frameBlendCount == 1)
                        //    // Use 10% of previous frame for some speckle-noise & flicker reduction
                        //    currImage = (0.9 * currImage.Mat + 0.1 * prevImage.Mat).ToImage<Bgr, byte>();
                        //else

                        // Smooth using multiple frames
                        currImage = frameSmoother.Update(currImage.ToBitmap()).ToImage<Bgr, byte>();

                        if (!maskSet)
                        {
                            using (Bitmap bmp = ShowEditMaskForm(currImage.ToBitmap(), mask))
                            {
                                maskSet = true;

                                if (bmp == null)
                                    continue;

                                mask = GetMatFromSDImage(bmp).ToImage<Bgra, byte>()[2]; // mask is red-channel
                                // Clean-up and invert to form the correct mask
                                var whiteImg = new Image<Gray, byte>(width, height);
                                whiteImg.SetValue(255);
                                mask = whiteImg.Copy(mask).Not();
                            }
                        }

                        bgBuilder.Apply(currImage, foregroundMask);
                        if (frameNumber == 0L)
                            background = currImage.Convert<Bgra, byte>();
                        else
                        {
                            Image<Bgra, byte> newBg = currImage.Convert<Bgra, byte>();
                            newBg[3].SetValue(1, foregroundMask.Not());
                            background[3].SetValue(1);
                            background = (background.Mat * .95 + newBg.Mat * .05).ToImage<Bgra, byte>();
                        }

                        if (leadInFrames > 4L)
                        {
                            leadInFrames--;
                            frameNumber++;
                            continue; // skip further processing until the lead-in is complete
                        }
                        else if (leadInFrames > 0L) 
                            leadInFrames--;

                        Mat foregroundMat = background.Convert<Bgr,byte>().Not().Mat + currImage.Mat;
                        currForeground = foregroundMat.ToImage<Bgr, byte>();
                        //currForeground = currImage.Copy(foregroundMask.Not());

                        Mat moveMat = currForeground.Mat - prevForeground.Mat;
                        movement = ((moveMat - processSettings.movementNoiseFloor) * processSettings.movementMultiplier).ToImage<Bgr, byte>().Convert<Gray,byte>();

                        if (mask != null)
                            movement = movement.Copy(mask);

                        currImage = new Image<Bgr, byte>(width, height, convertedFrame.linesize[0], (IntPtr)convertedFrame.data[0]);
                        prevImage.Bytes = currImage.Bytes;
                        prevForeground.Bytes = currForeground.Bytes;

                        if (leadInFrames == 0)
                        {
                            int currentFps = perfTimer.Update();
                            double processingRate = (double)currentFps / (double)videoFrameRate;
                            var time = TimeSpan.FromSeconds((double)frameNumber / (double)videoFrameRate);
                            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings
                            statusLabel.Text = $"{time:hh\\:mm\\:ss}";
                            statusProcessingRate.Text = $"Processing@{ processingRate: 0.0}x";
                            var moveScore = movement.GetSum().Intensity * processSettings.movementScoreMul;
                            if (framesSinceSeek == framesSinceSeekThresh)
                            {
                                movementScores[frameNumber] = (float)moveScore;
                                movementScoreMax = Math.Max(movementScoreMax, (float)moveScore);
                                UpdateChart();
                                // decay the maximum slowly to ensure early "noise" peaks don't destroy scaling forever
                                movementScoreMax *= 0.9999f;
                            }
                        }
                        processedFrameCount++;
                        frameNumber++;

                        if (!processMultithreaded)
                        {
                            Task.Run(() =>
                            {
                                Console.WriteLine("Task={0}, Thread={1}", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
                                UpdateProcessMainView(currImage.Mat);
                            });
                        }
                    }
                }
            }
        }

        private void UpdateProcessMainView(Mat currImageMat) 
        {
            movementHist = (movementHist.Mat * processSettings.movementHistoryDecay).ToImage<Gray, byte>();
            movementHist = (movementHist.Mat + movement.Mat).ToImage<Gray, byte>();

            // re-init to see un-processed input frame
            //currImage = new Image<Bgr, byte>(width, height, convertedFrame.linesize[0], (IntPtr)convertedFrame.data[0]);
            Image<Bgr, byte> moveImg = movementHist.Convert<Bgr, byte>();
            moveImg[0] = new Image<Gray, byte>(movementHist.Width, movementHist.Height); // Make the blue-channel zero
            moveImg[2] = new Image<Gray, byte>(movementHist.Width, movementHist.Height); // Make the red-channel zero
            //MethodInvoker m = new MethodInvoker(() => pictureBox1.Image = background.ToBitmap());
            //pictureBox1.Invoke(m);
            var picMI = new MethodInvoker(() => pictureBox1.Image = (0.7 * currImageMat + moveImg.Mat).ToImage<Bgr, byte>().ToBitmap());
            pictureBox1.Invoke(picMI);
        }

        private Bitmap ShowEditMaskForm(Bitmap bg, Image<Gray,byte> mask) {
            MaskForm form = new MaskForm();
            form.Background = bg;
            if (mask != null) 
            {
                Image<Bgra, byte> colourMask = new Image<Bgra, byte>(bg.Width, bg.Height, new Bgra(0, 0, 40.0, 200.0));
                form.Mask = colourMask.Copy(mask.Not()).ToBitmap();
            }
            if (form.ShowDialog() == DialogResult.OK) {
                return form.Mask; 
            }
            return null;
        }

        private Mat GetMatFromSDImage(Bitmap image)
        {
            int stride = 0;

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Imaging.BitmapData bmpData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, image.PixelFormat);

            System.Drawing.Imaging.PixelFormat pf = image.PixelFormat;
            if (pf == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                stride = image.Width * 4;
            }
            else
            {
                stride = image.Width * 3;
            }

            Image<Bgra, byte> cvImage = new Image<Bgra, byte>(image.Width, image.Height, stride, (IntPtr)bmpData.Scan0);

            image.UnlockBits(bmpData);

            return cvImage.Mat;
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

        private void openVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processingEnabled = false;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video files (avi,mov,mp4,mpg,mpeg,mkv,mts,wmv)|*.avi;" + 
                "*.Avi;*.AVI;*.mov;*.Mov;*.MOV;*.mp4;*.Mp4;*.MP4;*.mpg;*.Mpg;*.MPG;" + 
                "*.mpeg;*.Mpeg;*.MPEG;*.mkv;*.Mkv;*.MKV;*.mts;*.Mts;*.MTS;*.wmv;*.Wmv;*.WMV|" +
                "All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK) {
                string fname = Path.GetFileName(ofd.FileName);
                this.Text = "CAVAPA: " + fname + " v" + version.ToString();
                statusLabel.Text = fname;

                trackBar1.Value = 0;

                var mediaInfo = new MediaInfoDotNet.MediaFile(ofd.FileName);
                var videoInfo = mediaInfo.Video[0];
                var kbpsStr = "";
                int bps = 0;
                if (int.TryParse(videoInfo.bitRate, out bps))
                    kbpsStr = $" {bps/1000}kbps";
                statusVideoInfo.Text = $"{Path.GetExtension(ofd.FileName)} ({videoInfo.internetMediaType}{kbpsStr}) {videoInfo.Width}x{videoInfo.Height}@{videoInfo.frameRate}fps";
                Console.WriteLine("Video Format: " + statusVideoInfo.Text);

                videoFrameRate = videoInfo.frameRate;
                videoFrameCount = videoInfo.frameCount;
                trackBar1.Maximum = videoInfo.frameCount + 1;

                statusVideoDuration.Text = $"/{TimeSpan.FromMilliseconds(videoInfo.duration):hh\\:mm\\:ss}";

                if (videoInfo.Width >= 720)
                {
                    processSettings.frameBlendCount = 2;
                    enableFlickerReductionToolStripMenuItem.Checked = false;
                }
                else
                {
                    processSettings.frameBlendCount = 4;
                    enableFlickerReductionToolStripMenuItem.Checked = true;
                }

                processedFrameCount = 0L;
                movementScoreMax = 0;
                movementScores = new float[videoFrameCount];
                for (int i = 0; i < movementScores.Length; i++)
                {
                    movementScores[i] = float.NegativeInfinity;
                }

                var width = videoInfo.width;
                var height = videoInfo.height;
                movement = new Image<Gray, byte>(width, height);
                movementHist = new Image<Gray, byte>(width, height);

                Task.Run(() =>
                {
                    Console.WriteLine("Task={0}, Thread={1}", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
                    ProcessFrames(AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2, ofd.FileName);
                });

                //processMultithreaded = true;
                //int numChunks = Math.Min(Environment.ProcessorCount, 6); // Low-performance CPUs use under 4 threads
                //long frameStride = videoFrameCount / (long)numChunks;
                //long marker = 0L;
                //for (int i = 0; i < numChunks; i++)
                //{
                //    Task.Run(() =>
                //    {
                //        Console.WriteLine("Task={0}, Thread={1}", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
                //        ProcessFrames(AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2, ofd.FileName, i, marker, marker += frameStride);
                //    });
                //}
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutCAVAPAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.TextBoxContents += "\n\nVersion Info:\n\t" + versionInfo.Replace("|", "\n\t");
            aboutForm.ShowDialog();
        }

        private void editMaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            maskSet = false;
        }

        private void exportCSVDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (movementScores == null || movementScores.Count() == 0)
                return;

            processingSleep = true;

            SaveFileDialog sfd = new SaveFileDialog();
            if (File.Exists(csvExportPath))
                sfd.FileName = csvExportPath;
            else
                sfd.FileName = Path.GetFileNameWithoutExtension(videoFilepath) + ".csv";

            sfd.Filter = "Data files (csv,xls,xlsx)|*.csv;" +
                        "*.Csv;*.CSV;*.xls*;*.Xls*;*.XLS*|" +
                        "All files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                csvExportPath = sfd.FileName;

                bool saveSuccess = false;
                try
                {
                    using (TextWriter file = File.CreateText(csvExportPath))
                    {
                        file.WriteLine("FrameID, MovementScore");

                        var data = movementScores;
                        for (int i = 0; i < data.Length; i++)
                            file.WriteLine($"{i + 1},{data[i]:F3}");

                        file.Flush();
                        file.Close();
                    }
                    saveSuccess = true;
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                if (saveSuccess)
                    MessageBox.Show($"CSV data successfully exported to \"{csvExportPath}\". \n\n{movementScores.Length:#,##0} rows written", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            processingSleep = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            processingEnabled = false;
        }

        private void enableFlickerReductionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            menuItem.Checked = !menuItem.Checked;
            processSettings.frameBlendCount = (menuItem.Checked ? 4 : 2);
        }

        private void enableShadowReductionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            menuItem.Checked = !menuItem.Checked;
            processSettings.enableShadowReduction = menuItem.Checked;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            _trackBarPos = trackBar1.Value;
        }
    }
}
