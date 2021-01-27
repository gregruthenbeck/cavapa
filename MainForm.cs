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
using System.Diagnostics;
using System.Reflection;

namespace cavapa
{
    public partial class MainForm : Form
    {
        Version _version;
        string _versionInfo;
        string _videoFilepath = "";
        string _csvExportPath = "";

        bool _processingEnabled = true;
        bool _processingSleep = false;
        long _videoFrameCount = 0;
        float _videoFrameRate = 25.0f;
        float[] _movementScores = null;
        float _movementScoreMax = 0;
        bool _processMultithreaded = false;
        long _processedFrameCount = 0L;
        long _frameNumber = 0L;

        Image<Gray, byte> _movement;
        Image<Gray, byte> _movementHist;
        bool _maskSet = true;
        Image<Gray, byte> _mask = null;

        FPSTimer _perfTimer = null;
        Bitmap _bmpChart = null;
        PictureBox _pictureBoxChart = null;
        Form _settingsForm = null;
        //ProcessSettings _processSettings = new ProcessSettings();
        SettingsControl _settingsControl = null;

        private static int _trackBarPos = 0;

        public MainForm()
        {
            InitializeComponent();

            var execAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            _version = execAssembly.GetName().Version;
            _versionInfo = GetInformationalVersion(execAssembly);
            this.Text += " v" + _version.ToString() + "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
            Console.WriteLine("Running in {0}-bit mode.", Environment.Is64BitProcess ? "64" : "32");
            Console.WriteLine("Number of processors: {0}", Environment.ProcessorCount);
            FFmpegBinariesHelper.RegisterFFmpegBinaries();
            Console.WriteLine($"FFmpeg version info: {ffmpeg.av_version_info()}");
            SetupLogging();

            // shorten the bright-green glow-trail of movements (default is 0.9)
            _settingsControl = new SettingsControl();
            _settingsControl.MovementHistoryDecay = 0.85; 
            //processSettings.frameBlendCount = 2;
            //processSettings.movementMultiplier = 20.0;

            // Test BGBuilder
            //foreach (var f in Directory.GetFiles(".", "bg*.jpg"))
            //    File.Delete(f);
            //var bgb = new BGBuilder(AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2, "../../../CameraB_cut.mp4");
            //var bgb = new BGBuilder(AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2, "../../../kilp_2011_8_22-10-koris-deint.mp4");

            //enableFlickerReductionToolStripMenuItem.Checked = (_processSettings.frameBlendCount > 1);
            //enableShadowReductionToolStripMenuItem.Checked = _settingsControl.EnableShadowReduction;

            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            _pictureBoxChart = new PictureBox();
            _bmpChart = new Bitmap(this.tableLayoutPanel1.Size.Width, 120, PixelFormat.Format32bppArgb);
            _pictureBoxChart.Size = new Size(_bmpChart.Width, _bmpChart.Height);
            _pictureBoxChart.Margin = new Padding(0);
            this.tableLayoutPanel1.Controls.Add(this._pictureBoxChart, 0, 3);

            UpdateRecentItems();

            _settingsForm = new Form();
            _settingsForm.Controls.Add(_settingsControl);
            _settingsForm.Controls[0].Dock = DockStyle.Fill;
            _settingsForm.Size = new Size(409, 320);
            _settingsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            _settingsForm.MaximizeBox = false;
            _settingsForm.MinimizeBox = false;
            _settingsForm.Text = "CAVAPA Settings";
            _settingsForm.StartPosition = FormStartPosition.CenterParent;
            _settingsForm.CancelButton = _settingsControl.CloseButton;

            string[] args = Environment.GetCommandLineArgs();
            if (args != null && args.Length > 1 &&
                File.Exists(args[1])) {
                try {
                    if (args.Length > 2 && File.Exists(args[2])) {
                        string ext = Path.GetExtension(args[2]).ToLower();
                        if (ext == ".txt" || ext == ".cfg") {
                            LoadSettings(args[2]);
                        }
                    }
                    OpenVideo(args[1]);
                } catch { 
                }
            }
        }

        private void UpdateRecentItems() 
        {
            if (Properties.Settings.Default.RecentVideoFilePaths != null)
            {
                ToolStripMenuItem[] recentVideos = new ToolStripMenuItem[Properties.Settings.Default.RecentVideoFilePaths.Count];
                for (int i = 0; i < recentVideos.Length; i++)
                {
                    recentVideos[i] = new ToolStripMenuItem("&" + (i + 1).ToString() + " " + Path.GetFileName(Properties.Settings.Default.RecentVideoFilePaths[i]));
                    recentVideos[i].Click += OpenRecentVideo_Click;
                }
                recentOneToolStripMenuItem.DropDownItems.Clear();
                recentOneToolStripMenuItem.DropDownItems.AddRange(recentVideos);
            }
        }

        private void OpenRecentVideo_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            string idStr = menuItem.Text.Split(new char[] { ' ' })[0].Replace("&","");
            int id = 0;
            if (int.TryParse(idStr, out id))
                OpenVideo(Properties.Settings.Default.RecentVideoFilePaths[id-1]);
        }

        private void OpenVideo(string filepath)
        {
            if (!File.Exists(filepath)) {
                MessageBox.Show("The selected video file does not exist.\n" + filepath, "File not found!");
                return;
            }

            if (_processingEnabled)
            {
                _processingEnabled = false;
                Thread.Sleep(500);
            }

            string fname = Path.GetFileName(filepath);
            this.Text = "CAVAPA: " + fname + " v" + _version.ToString();
            statusLabel.Text = fname;

            if (Properties.Settings.Default.RecentVideoFilePaths == null)
                Properties.Settings.Default.RecentVideoFilePaths = new System.Collections.Specialized.StringCollection();

            if (!Properties.Settings.Default.RecentVideoFilePaths.Contains(filepath))
            {
                Properties.Settings.Default.RecentVideoFilePaths.Insert(0, filepath);
                if (Properties.Settings.Default.RecentVideoFilePaths.Count > 12)
                    Properties.Settings.Default.RecentVideoFilePaths.RemoveAt(12);

                UpdateRecentItems();
            }

            var mediaInfo = new MediaInfoDotNet.MediaFile(filepath);
            var videoInfo = mediaInfo.Video[0];
            var kbpsStr = "";
            int bps = 0;
            if (int.TryParse(videoInfo.bitRate, out bps))
                kbpsStr = $" {bps / 1000}kbps";
            statusVideoInfo.Text = $"{Path.GetExtension(filepath)} ({videoInfo.internetMediaType}{kbpsStr}) {videoInfo.Width}x{videoInfo.Height}@{videoInfo.frameRate}fps";
            Console.WriteLine("Video Format: " + statusVideoInfo.Text);

            _videoFrameRate = videoInfo.frameRate;
            _videoFrameCount = videoInfo.frameCount;

            _settingsControl.EnableDespeckle = videoInfo.height < 720; // disable de-speckle for HD videos (speed)

            trackBar1.Value = 0;
            trackBar1.Maximum = videoInfo.frameCount;
            statusVideoDuration.Text = $"/{TimeSpan.FromMilliseconds(videoInfo.duration):hh\\:mm\\:ss}";

            _processedFrameCount = 0L;
            _movementScoreMax = 0;
            _movementScores = new float[_videoFrameCount + 1];
            for (int i = 0; i < _movementScores.Length; i++) {
                _movementScores[i] = float.NegativeInfinity;
            }

            var width = videoInfo.width;
            var height = videoInfo.height;
            _movement = new Image<Gray, byte>(width, height);
            _movementHist = new Image<Gray, byte>(width, height);

            Task.Run(() =>
            {
                Console.WriteLine("Task={0}, Thread={1}", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
                ProcessFrames(AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2, filepath, 0, 0, _videoFrameCount - 1);
            });
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
            int interval = Math.Max((int)(_videoFrameCount / (2L * (long)tableLayoutPanel1.Width)), 1); // Max(,1) to handle short videos

            if (_frameNumber % interval != 0 ||
                _frameNumber / interval < 2)
                return;

            PointF[] points = new PointF[_videoFrameCount / interval];
            SizeF s = new SizeF(.5f, (float)_bmpChart.Height / _movementScoreMax);
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PointF((float)i * s.Width, _bmpChart.Height - _movementScores[i * interval] * s.Height);
                points[i].Y = Math.Min(points[i].Y, (float)_bmpChart.Height-2.0f);
            }
            points[0] = new PointF(0F, (float)(_bmpChart.Height - 1));

            using (Graphics g = Graphics.FromImage(_bmpChart))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.FillRectangle(Brushes.LightGray, 0, 0, _bmpChart.Width, _bmpChart.Height);
                g.DrawLines(Pens.Blue, points);
            }

            MethodInvoker m = new MethodInvoker(() => _pictureBoxChart.Image = _bmpChart);
            _pictureBoxChart.Invoke(m);
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
            _videoFilepath = url;
            _processingEnabled = true;
            _perfTimer = new FPSTimer((int)_videoFrameRate);
            long leadInFrames = 5;
            if (!_processMultithreaded && leadInFrames < startFrame) // dont't do leadIn if we're too close to the start of the video
                startFrame = startFrame - leadInFrames;
            else
                leadInFrames = 0L;

            // Don't set endFrame in most cases since we want to be able to seek to end and not have this loop exit
            //if (endFrame > _videoFrameCount)
            //    endFrame = _videoFrameCount - 1;

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
                    _frameNumber = startFrame;
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
                    //FrameBlender frameSmoother = new FrameBlender(width, height, _processSettings.frameBlendCount);
                    Image<Bgra, byte> background = null;
                    var bgBuilder = new Emgu.CV.BackgroundSubtractorMOG2(250, 16, false);
                    Image<Gray, byte> foregroundMask = new Image<Gray, byte>(width, height);
                    var currForeground = new Image<Bgr, byte>(width, height);
                    var prevForeground = new Image<Bgr, byte>(width, height);
                    //var movement = new Image<Gray, byte>(width, height);
                    //var movementHist = new Image<Gray, byte>(width, height);

                    bool decoderActive = true;
                    AVFrame frame;
                    while (decoderActive && (_frameNumber < endFrame) && _processingEnabled)
                    {
                        int seekFrameNum = _trackBarPos;
                        if (!_processMultithreaded)
                        {
                            if (Math.Abs(_frameNumber - seekFrameNum) > 250)
                            {
                                vsd.Seek(seekFrameNum);
                                _frameNumber = seekFrameNum;
                                framesSinceSeek = 0;
                            }

                            var trackBarSetMI = new MethodInvoker(() => trackBar1.Value = Math.Min((int)_frameNumber, trackBar1.Maximum - 1));
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

                        while (_processingSleep)
                            Thread.Sleep(500);

                        var convertedFrame = vfc.Convert(frame);

                        Image<Bgr, byte> currImage = new Image<Bgr, byte>(width, height, convertedFrame.linesize[0], (IntPtr)convertedFrame.data[0]);
                        // Shadow reduction: Shadows are lindear and less vertical, so stretch the image wider
                        // and then resize back to original aspect-ratio to discard some horizontal detail.
                        // Also, people are taller than bikes & balls
                        if (_settingsControl.EnableShadowReduction)
                            currImage = currImage.Resize(width, height / 8, Emgu.CV.CvEnum.Inter.Area).Resize(width, height, Emgu.CV.CvEnum.Inter.Area);

                        // Smooth using multiple frames
                        // Use 10% of previous frame for some speckle-noise & flicker reduction
                        if (_settingsControl.EnableDespeckle)
                            currImage = currImage.SmoothGaussian(3);
                        currImage = (0.3 * currImage.Mat + 0.7 * prevImage.Mat).ToImage<Bgr, byte>();
                        //currImage = frameSmoother.Update(currImage.ToBitmap()).ToImage<Bgr, byte>();

                        if (!_maskSet)
                        {
                            using (Bitmap bmp = ShowEditMaskForm(currImage.ToBitmap(), _mask))
                            {
                                _maskSet = true;

                                if (bmp == null)
                                    continue;

                                _mask = GetMatFromSDImage(bmp).ToImage<Bgra, byte>()[2]; // mask is red-channel
                                // Clean-up and invert to form the correct mask
                                var whiteImg = new Image<Gray, byte>(width, height);
                                whiteImg.SetValue(255);
                                _mask = whiteImg.Copy(_mask).Not();
                            }
                        }

                        bgBuilder.Apply(currImage, foregroundMask);
                        if (_frameNumber == 0L)
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
                            _frameNumber++;
                            continue; // skip further processing until the lead-in is complete
                        }
                        else if (leadInFrames > 0L) 
                            leadInFrames--;

                        Mat foregroundMat = background.Convert<Bgr,byte>().Not().Mat + currImage.Mat;
                        currForeground = foregroundMat.ToImage<Bgr, byte>();
                        if (_settingsControl.EnableDespeckle)
                            currForeground = currForeground.SmoothGaussian(3); // remove speckle (video low-light noise, small birds, insects, etc)
                        //currForeground = currImage.Copy(foregroundMask.Not());

                        Mat moveMat = currForeground.Mat - prevForeground.Mat;
                        _movement = ((moveMat - _settingsControl.MovementNoiseFloor) * _settingsControl.MovementPixMul).ToImage<Bgr, byte>().Convert<Gray,byte>();

                        if (_mask != null)
                            _movement = _movement.Copy(_mask);

                        currImage = new Image<Bgr, byte>(width, height, convertedFrame.linesize[0], (IntPtr)convertedFrame.data[0]);
                        prevImage.Bytes = currImage.Bytes;
                        prevForeground.Bytes = currForeground.Bytes;

                        if (leadInFrames == 0)
                        {
                            int currentFps = _perfTimer.Update();
                            double processingRate = (double)currentFps / (double)_videoFrameRate;
                            var time = TimeSpan.FromSeconds((double)_frameNumber / (double)_videoFrameRate);
                            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings
                            statusLabel.Text = $"{time:hh\\:mm\\:ss}";
                            statusProcessingRate.Text = $"Processing@{ processingRate: 0.0}x";
                            var moveScore = _movement.GetSum().Intensity * _settingsControl.MovementScoreMul;
                            if (framesSinceSeek == framesSinceSeekThresh)
                            {
                                if (_frameNumber < _movementScores.Length)
                                    _movementScores[_frameNumber] = (float)moveScore;
                                _movementScoreMax = Math.Max(_movementScoreMax, (float)moveScore);
                                UpdateChart();
                                // decay the maximum slowly to ensure early "noise" peaks don't destroy scaling forever
                                _movementScoreMax *= 0.9999f;
                            }
                        }
                        _processedFrameCount++;
                        _frameNumber++;

                        if (!_processMultithreaded && (_frameNumber < endFrame))
                        {
                            // Off-load some processing to another thread to allow faster updates on the main processing thread
                            Task.Run(() =>
                            {
                                try
                                {
                                    UpdateProcessMainView(currImage.Mat);
                                }
                                catch { }
                            });
                        }

                        if (_frameNumber == endFrame) {
                            var mi = new MethodInvoker(() => ProcessingCompleteAndExport());
                            this.Invoke(mi);
                        }
                    }
                }
            }
        }

        private void ProcessingCompleteAndExport() 
        {
            var successForm = new ProcessingCompleteForm();
            successForm.Filename = Path.GetFileName(_videoFilepath);
            if ((_processedFrameCount > (long)(0.98 * (double)_videoFrameCount)) &&
                 successForm.ShowDialog() == DialogResult.OK) {
                exportCSVDataFileToolStripMenuItem_Click(this, new ProcessingCompletedEventArgs(successForm.OpenAfterExportEnabled));
            }

        }

        private void UpdateProcessMainView(Mat currImageMat) 
        {
            _movementHist = (_movementHist.Mat * _settingsControl.MovementHistoryDecay).ToImage<Gray, byte>();
            _movementHist = (_movementHist.Mat + _movement.Mat).ToImage<Gray, byte>();

            Image<Bgr, byte> moveImg = _movementHist.Convert<Bgr, byte>();
            moveImg[0] = new Image<Gray, byte>(_movementHist.Width, _movementHist.Height); // Make the blue-channel zero
            moveImg[2] = new Image<Gray, byte>(_movementHist.Width, _movementHist.Height); // Make the red-channel zero

            try
            {
                if (_processingEnabled)
                {
                    var picMI = new MethodInvoker(() => pictureBox1.Image = (0.7 * currImageMat + moveImg.Mat).ToImage<Bgr, byte>().ToBitmap());
                    pictureBox1.Invoke(picMI);
                }
            }
            catch { }
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
            _processingEnabled = false;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video files (avi,mov,mp4,mpg,mpeg,mkv,mts,wmv)|*.avi;" + 
                "*.Avi;*.AVI;*.mov;*.Mov;*.MOV;*.mp4;*.Mp4;*.MP4;*.mpg;*.Mpg;*.MPG;" + 
                "*.mpeg;*.Mpeg;*.MPEG;*.mkv;*.Mkv;*.MKV;*.mts;*.Mts;*.MTS;*.wmv;*.Wmv;*.WMV|" +
                "All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK) {
                OpenVideo(ofd.FileName);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutCAVAPAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.TextBoxContents += "\n\nVersion Info:\n\t" + _versionInfo.Replace("|", "\n\t");
            aboutForm.ShowDialog();
        }

        private void editMaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _maskSet = false;
        }

        private void exportCSVDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_movementScores == null || _movementScores.Count() == 0)
                return;

            _processingSleep = true;

            SaveFileDialog sfd = new SaveFileDialog();
            if (File.Exists(_csvExportPath))
                sfd.FileName = _csvExportPath;
            else
                sfd.FileName = Path.GetFileNameWithoutExtension(_videoFilepath) + ".csv";

            sfd.Filter = "Data files (csv,xls,xlsx)|*.csv;" +
                        "*.Csv;*.CSV;*.xls*;*.Xls*;*.XLS*|" +
                        "All files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _csvExportPath = sfd.FileName;

                bool saveSuccess = false;
                try
                {
                    using (TextWriter file = File.CreateText(_csvExportPath))
                    {
                        file.WriteLine("FrameID, MovementScore");

                        var data = _movementScores;
                        for (int i = 0; i < data.Length; i++)
                        {
                            float d = data[i];
                            if (!(d > 0 && d < 1E10f))
                                d = 0;
                            file.WriteLine($"{i + 1},{d:F3}");
                        }

                        file.Flush();
                        file.Close();
                    }
                    saveSuccess = true;
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                bool openInExcel = false;
                try {
                    var args = e as ProcessingCompletedEventArgs;
                    openInExcel = args.OpenAfterExport;
                } catch {
                }

                if (saveSuccess && !openInExcel)
                    MessageBox.Show($"CSV data successfully exported to \"{_csvExportPath}\". \n\n{_movementScores.Length:#,##0} rows written", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (openInExcel) {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = _csvExportPath;
                    proc.Start();
                }
            }
            _processingSleep = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            _processingEnabled = false;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            _trackBarPos = trackBar1.Value;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingsForm.ShowDialog();
        }

        private void exportMaskToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_mask == null) {
                MessageBox.Show("No mask has been set.", "Export failed");
                return;
            }

            try {
                _processingSleep = true;
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Mask (png)|*.png;*.Png;*.PNG;*.png;*.Png;*.PNG|" +
                        "All files (*.*)|*.*";
                dlg.InitialDirectory = Path.GetDirectoryName(_videoFilepath);
                dlg.FileName = Path.GetDirectoryName(_videoFilepath) + "\\" +
                    Path.GetFileNameWithoutExtension(_videoFilepath) + "-mask.png";
                if (dlg.ShowDialog() == DialogResult.OK) {
                    _mask.Save(dlg.FileName);
                }
            } 
            finally {
                _processingSleep = false;
            }
        }

        private void importMaskToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                _processingSleep = true;
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Mask (png)|*.png;*.Png;*.PNG;*.png;*.Png;*.PNG|" +
                        "All files (*.*)|*.*";
                if (File.Exists(_videoFilepath)) {
                    dlg.InitialDirectory = Path.GetDirectoryName(_videoFilepath);
                    dlg.FileName = Path.GetDirectoryName(_videoFilepath) + "\\" +
                        Path.GetFileNameWithoutExtension(_videoFilepath) + "-mask.png";
                }
                if (dlg.ShowDialog() == DialogResult.OK) {
                    _mask = new Image<Gray, byte>(dlg.FileName);
                }
            } finally {
                _processingSleep = false;
            }
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                _processingSleep = true;
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Settings (cfg,txt)|*.cfg;*.Cfg;*.CFG;*.txt;*.Txt;*.TXT|" +
                        "All files (*.*)|*.*";
                if (File.Exists(_videoFilepath)) {
                    dlg.InitialDirectory = Path.GetDirectoryName(_videoFilepath);
                    dlg.FileName = Path.GetDirectoryName(_videoFilepath) + "\\" +
                        Path.GetFileNameWithoutExtension(_videoFilepath) + "-settings.txt";
                }
                if (dlg.ShowDialog() == DialogResult.OK) {
                    _settingsControl.SaveSettings(dlg.FileName);
                }
            } finally {
                _processingSleep = false;
            }
        }

        private void LoadSettings(string filepath) {
            try {
                _processingSleep = true;
                _settingsControl.LoadSettings(filepath);
            } catch {
            } finally {
                _processingSleep = false;
            }
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                _processingSleep = true;
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Settings (cfg,txt)|*.cfg;*.Cfg;*.CFG;*.txt;*.Txt;*.TXT|" +
                        "All files (*.*)|*.*";
                if (File.Exists(_videoFilepath)) {
                    dlg.FileName = Path.GetDirectoryName(_videoFilepath) + "\\" + 
                        Path.GetFileNameWithoutExtension(_videoFilepath) + "-settings.txt";
                }
                if (dlg.ShowDialog() == DialogResult.OK) {
                    _settingsControl.LoadSettings(dlg.FileName);
                }
            } finally {
                _processingSleep = false;
            }
        }
    }
}
