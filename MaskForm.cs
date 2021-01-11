using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cavapa
{
    public partial class MaskForm : Form
    {
        Timer paintTimer;

        Bitmap background, mask, bmp;

        bool mouseLButtonDown = false;
        bool mouseRButtonDown = false;
        Point mousePos;
        int brushRadius = 80;

        public Bitmap Background
        {
            set
            {
                background = value;
                mask = new Bitmap(value.Width, value.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bmp = new Bitmap(value.Width, value.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            get
            {
                return background;
            }
        }

        public Bitmap Mask
        {
            get
            {
                return mask;
            }
        }

        public MaskForm()
        {
            InitializeComponent();
        }

        private void MaskForm_Load(object sender, EventArgs e)
        {
            paintTimer = new Timer();
            paintTimer.Tick += PaintTimer_Tick;
            paintTimer.Interval = 1000 / 40; // 40Hz
            paintTimer.Start();
        }

        private void PaintTimer_Tick(object sender, EventArgs e)
        {
            Point p = mousePos;
            int r = brushRadius;
            Rectangle brushRect = new Rectangle(p.X - r, p.Y - r, 2 * r, 2 * r);

            if (mouseLButtonDown || mouseRButtonDown) {
                using (Graphics g = Graphics.FromImage(mask))
                {
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    if (mouseLButtonDown)
                        g.FillEllipse(new SolidBrush(Color.FromArgb(160, Color.Red)), brushRect);
                    else
                        g.FillEllipse(new SolidBrush(Color.Transparent), brushRect);
                }
            }

            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawImageUnscaled(background, 0, 0);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.DrawImageUnscaled(mask, 0, 0);
                g.DrawEllipse(new Pen(Color.FromArgb(200,Color.Green),2.0f), brushRect);
            }

            pictureBox1.Image = bmp;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLButtonDown = (e.Button == MouseButtons.Left);
            mouseRButtonDown = (e.Button == MouseButtons.Right);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseLButtonDown = false;
            mouseRButtonDown = false;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            mouseLButtonDown = false;
            mouseRButtonDown = false;
        }
    }
}
