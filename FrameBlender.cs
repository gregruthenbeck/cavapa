using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cavapa
{
    class FrameBlender
    {
        private Mat[] frames;
        private Mat frameSum;
        private int loopIndex;

        public FrameBlender(int w, int h, int numFrames)
        {
            loopIndex = 0;

            frames = new Mat[numFrames];
            for (int i = 0; i < numFrames; i++)
            {
                frames[i] = new Image<Bgr, byte>(w, h).Mat;
            }
            frameSum = new Image<Bgr, byte>(w, h).Mat;
        }

        public Mat Update(Mat frame) 
        {
            ++loopIndex;
            if (loopIndex == frames.Length)
                loopIndex = 0;

            frame.CopyTo(frames[loopIndex]);

            frameSum.SetTo(new MCvScalar(0));
            for (int i = 0; i < frames.Length; i++)
            {
                frameSum += frames[i] * (1.0 / (double)frames.Length);
            }
            return frameSum;
        }
    }
}
