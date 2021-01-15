using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cavapa
{
    class FrameBlender
    {
        private Bitmap bmp;
        private ImageAttributes imageAttributes;
        private bool firstFrame = true;
        //private ImageAttributes[] imageAttrRGB;

        public FrameBlender(int w, int h, int numFrames)
        {
            bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-use-a-color-matrix-to-set-alpha-values-in-images
            float opacity = 1.0f / numFrames;
            float[][] matrixItems ={
                                new float[] {1, 0, 0, 0, 0},
                                new float[] {0, 1, 0, 0, 0},
                                new float[] {0, 0, 1, 0, 0},
                                new float[] {0, 0, 0, opacity, 0},
                                new float[] {0, 0, 0, 0, 1}};
            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
            imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            //imageAttrRGB = new ImageAttributes[3];
            //for (int i = 0; i < 3; i++)
            //    imageAttrRGB[i] = new ImageAttributes();
            //matrixItems[0][0] = 1; matrixItems[1][1] = 0; matrixItems[2][2] = 0;
            //imageAttrRGB[0].SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            //matrixItems[0][0] = 0; matrixItems[1][1] = 1; matrixItems[2][2] = 0;
            //imageAttrRGB[1].SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            //matrixItems[0][0] = 0; matrixItems[1][1] = 0; matrixItems[2][2] = 1;
            //imageAttrRGB[2].SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        public Bitmap Update(Bitmap frame) 
        {
            using (Graphics g = Graphics.FromImage(bmp)) 
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                if (firstFrame) {
                    firstFrame = false;
                    g.DrawImageUnscaled(frame, 0, 0);
                }

                g.DrawImage(
                   frame,
                   new Rectangle(0, 0, frame.Width, frame.Height),  // destination rectangle
                   0.0f,                          // source rectangle x
                   0.0f,                          // source rectangle y
                   frame.Width,                   // source rectangle width
                   frame.Height,                  // source rectangle height
                   GraphicsUnit.Pixel,
                   imageAttributes);
            }
            return bmp;
        }
    }
}
