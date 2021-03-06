﻿using System;
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

        SizeF initScale;
        float zoomRate = 1.1f;
        bool mouseLButtonDown = false;
        bool mouseRButtonDown = false;
        Point mousePos, mouseDownPos, mouseUpPos;
        int brushRadius = 80;
        float brushSizeChangeRate = 1.1f;
        bool hasChanged = false;
        bool smoothClose = false;

        public Bitmap Background
        {
            set
            {
                background = value;
                mask = new Bitmap(value.Width, value.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bmp = new Bitmap(value.Width, value.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                if (value.Height < 720) {
                    initScale = new SizeF(3f, 3f);
                } else if (value.Height < 1080) {
                    initScale = new SizeF(1.5f, 1.5f);
                } else {
                    initScale = new SizeF(1, 1);
                }
                pictureBox1.Size = new Size((int)(initScale.Width * (float)value.Width),
                                            (int)(initScale.Height * (float)value.Height));
                this.Width = pictureBox1.Width + 20;
                this.Height = pictureBox1.Height + 90;
            }
            get
            {
                return background;
            }
        }

        public Bitmap Mask
        {
            set 
            {
                mask = value;
            }
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
            paintTimer.Interval = 1000 / 100; // 40Hz
            paintTimer.Start();
        }

        PointF TransformCoords(Point pos) 
        {
            PointF p = new PointF(pos.X, pos.Y);
            SizeF windowScale = new SizeF((float)background.Width / (float)pictureBox1.Width, (float)background.Height / (float)pictureBox1.Height);
            p.X *= windowScale.Width;
            p.Y *= windowScale.Height;
            return p;
        }

        private void PaintTimer_Tick(object sender, EventArgs e)
        {
            PointF p = TransformCoords(mousePos);
            SizeF windowScale = new SizeF((float)background.Width / (float)pictureBox1.Width, (float)background.Height / (float)pictureBox1.Height);
            PointF r = new PointF(brushRadius * windowScale.Width, brushRadius * windowScale.Height);
            RectangleF brushRect = new RectangleF(p.X - r.X, p.Y - r.Y, 2 * r.X, 2 * r.Y);

            if (!ModifierKeys.HasFlag(Keys.Control) && (mouseLButtonDown || mouseRButtonDown)) {
                using (Graphics g = Graphics.FromImage(mask))
                {
                    //g.Transform.Scale(windowScale.Width, windowScale.Height); // TODO: Use this coordinate-transform of the graphics context
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    if (mouseLButtonDown)
                        g.FillEllipse(new SolidBrush(Color.FromArgb(200, 40, 0, 0)), brushRect);
                    else
                        g.FillEllipse(new SolidBrush(Color.Transparent), brushRect);
                    hasChanged = true;
                }
            }

            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawImageUnscaled(background, 0, 0);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.DrawImageUnscaled(mask, 0, 0);

                if (mouseDownPos != null && (ModifierKeys.HasFlag(Keys.Control) || mouseUpPos != null))
                {
                    Point minima = new Point();
                    Point maxima = new Point();
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        minima.X = Math.Min(mouseDownPos.X, mousePos.X);
                        minima.Y = Math.Min(mouseDownPos.Y, mousePos.Y);
                    }
                    else
                    {
                        minima.X = Math.Min(mouseDownPos.X, mouseUpPos.X);
                        minima.Y = Math.Min(mouseDownPos.Y, mouseUpPos.Y);
                    }
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        maxima.X = Math.Max(mouseDownPos.X, mousePos.X);
                        maxima.Y = Math.Max(mouseDownPos.Y, mousePos.Y);
                    }
                    else
                    {
                        maxima.X = Math.Max(mouseDownPos.X, mouseUpPos.X);
                        maxima.Y = Math.Max(mouseDownPos.Y, mouseUpPos.Y);
                    }
                    if (minima.X >0 || minima.Y > 0)
                        g.DrawRectangle(new Pen(Color.Green, 2.0f), minima.X, minima.Y, maxima.X - minima.X, maxima.Y - minima.Y);
                }

                g.DrawEllipse(new Pen(Color.FromArgb(127, Color.White), 1.0f), brushRect);
                brushRect.X += 1.0f;
                brushRect.Y += 1.0f;
                brushRect.Width -= 1.0f;
                brushRect.Height -= 1.0f;
                g.DrawEllipse(new Pen(Color.FromArgb(127, Color.Black), 1.0f), brushRect);
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

            if (ModifierKeys.HasFlag(Keys.Control))
                mouseDownPos = mousePos;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseLButtonDown = false;
            mouseRButtonDown = false;

            if (ModifierKeys.HasFlag(Keys.Control))
                mouseUpPos = mousePos;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            mouseLButtonDown = false;
            mouseRButtonDown = false;
        }

        private void buttonZoomReset_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)(initScale.Width * (float)background.Width);
            pictureBox1.Height = (int)(initScale.Height * (float)background.Height);
        }

        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)(zoomRate * (float)pictureBox1.Width);
            pictureBox1.Height = (int)(zoomRate * (float)pictureBox1.Height);
        }

        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)((float)pictureBox1.Width / zoomRate);
            pictureBox1.Height = (int)((float)pictureBox1.Height / zoomRate);
        }

        private void buttonBrushReset_Click(object sender, EventArgs e)
        {
            brushRadius = 80;
        }

        private void buttonResetROI_Click(object sender, EventArgs e)
        {
            mouseDownPos = new Point(0, 0);
            mouseUpPos = new Point(0, 0);
        }

        private void MaskForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!hasChanged)
            {
                paintTimer.Stop();
                DialogResult = DialogResult.Cancel;
                return;
            }

            if (smoothClose) {
                paintTimer.Stop();
                DialogResult = DialogResult.OK;
                return;
            }

            if (MessageBox.Show("You have unsaved changes. Continue without saving?", "CAVAPA Mask: Unsaved changes",
                MessageBoxButtons.OKCancel) == DialogResult.Cancel) {
                e.Cancel = true;
                buttonSaveAndClose.BackColor = Color.LightGreen;
                buttonSaveAndClose.Focus();
            }
        }

        private void buttonSaveAndClose_Click(object sender, EventArgs e)
        {
            smoothClose = true;
            this.Close();
        }

        private void buttonBrushSmaller_Click(object sender, EventArgs e)
        {
            brushRadius = (int)(brushRadius / brushSizeChangeRate);
        }

        private void buttonBrushBigger_Click(object sender, EventArgs e)
        {
            brushRadius = (int)(brushRadius * brushSizeChangeRate);
        }
    }
}
