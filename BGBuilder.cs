using Emgu.CV;
using Emgu.CV.Structure;
using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cavapa
{
    public unsafe class BGBuilder
    {
        public BGBuilder(AVHWDeviceType HWDevice, string url)
        {
            var mediaInfo = new MediaInfoDotNet.MediaFile(url);
            var videoInfo = mediaInfo.Video[0];
            var videoFrameRate = videoInfo.frameRate; // Hz (fps)
            var videoFrameCount = videoInfo.frameCount;
            int bgFrameCount = 0;
            long bgFrameInterval = 5 * (long)videoInfo.frameRate; // in frames
            long frameCount = 0L;

            using (var vsd = new VideoStreamDecoder(url, HWDevice))
            {
                var srcSize = vsd.FrameSize;
                var srcPixelFormat = HWDevice == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE ? vsd.PixelFormat : GetHWPixelFormat(HWDevice);
                var dstSize = srcSize;
                var dstPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                var width = dstSize.Width;
                var height = dstSize.Height;
                List<Bitmap> frames = new List<Bitmap>();

                using (var vfc = new VideoFrameConverter(srcSize, srcPixelFormat, dstSize, dstPixelFormat))
                {
                    // Every 30s, vote on most "unchanged" pixel colours (every 5 seconds = 6 images) and assemble the BG
                    // Store the voted-unchanged-colour images
                    while (vsd.TryDecodeNextFrame(out var frame))
                    {

                        if (frameCount % bgFrameInterval == 0)
                        {
                            var convertedFrame = vfc.Convert(frame);
                            Bitmap currImage = new Bitmap(width, height, convertedFrame.linesize[0], PixelFormat.Format24bppRgb, (IntPtr)convertedFrame.data[0]);
                            //currImage.Save($"bg{bgFrameCount:D6}.jpg", ImageFormat.Jpeg);

                            frames.Add(new Bitmap(currImage));
                            int numFrames = 20;
                            if (frames.Count == numFrames) {
                                Rectangle rect = new Rectangle(0, 0, currImage.Width, currImage.Height);
                                Bitmap bgDst = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
                                BitmapData bgDstData = bgDst.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                BitmapData[] bmpDatas = new BitmapData[numFrames];
                                byte*[] p = new byte*[numFrames];
                                int colourDen = 12;
                                int colourDim = 256 / colourDen;
                                for (int i = 0; i < numFrames; i++)
                                {
                                    bmpDatas[i] = frames[i].LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                                    p[i] = (byte*)bmpDatas[i].Scan0.ToPointer();
                                }

                                byte* d = (byte*)bgDstData.Scan0.ToPointer();
                                for (int yi = 0; yi < rect.Height; yi++)
                                {
                                    for (int xi = 0; xi < rect.Width; xi++, d+=3)
                                    {
                                        SortedDictionary<int, List<Color> > votes = new SortedDictionary<int, List<Color> >();
                                        for (int k = 0; k < numFrames; k++)
                                        {
                                            Color c = Color.FromArgb(*(p[k] + 0), *(p[k] + 1), *(p[k] + 2));
                                            // 3D Color-space 256/3 ^3
                                            int colorKey = (c.R / colourDen) +
                                                           (c.G / colourDen) * colourDim +
                                                           (c.B / colourDen) * colourDim * colourDim;

                                            if (!votes.ContainsKey(colorKey))
                                                votes[colorKey] = new List<Color>();    
                                            votes[colorKey].Add(c);

                                            p[k] += 3;
                                        }

                                        int maxVotes = int.MinValue;
                                        int keyOfMaxVotes = -1;
                                        foreach (var key in votes.Keys)
                                        {
                                            if (votes[key].Count > maxVotes) 
                                            {
                                                maxVotes = votes[key].Count;
                                                keyOfMaxVotes = key;
                                            }
                                        }

                                        int[] sum = new int[3];
                                        for (int k = 0; k < 3; k++)
                                            sum[k] = 0;

                                        for (int k = 0; k < votes[keyOfMaxVotes].Count; k++)
                                        {
                                            sum[0] += votes[keyOfMaxVotes][k].R;
                                            sum[1] += votes[keyOfMaxVotes][k].G;
                                            sum[2] += votes[keyOfMaxVotes][k].B;
                                        }
                                        //if (votes[keyOfMaxVotes].Count != numFrames)
                                        //    Console.WriteLine("Interesting");

                                        *(d + 0) = (byte)((float)sum[0] / (float)votes[keyOfMaxVotes].Count);
                                        *(d + 1) = (byte)((float)sum[1] / (float)votes[keyOfMaxVotes].Count);
                                        *(d + 2) = (byte)((float)sum[2] / (float)votes[keyOfMaxVotes].Count);
                                    }
                                }

                                for (int i = 0; i < 6; i++)
                                {
                                    frames[i].UnlockBits(bmpDatas[i]);
                                }
                                bgDst.UnlockBits(bgDstData);
                                bgDst.Save($"bgc{frameCount:D06}.jpg", ImageFormat.Jpeg);

                                frames.Clear();
                            }

                            bgFrameCount++;
                        }

                        frameCount++;
                    }
                }
            }
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
    }
}
