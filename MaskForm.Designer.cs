
namespace cavapa
{
    partial class MaskForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonZoomReset = new System.Windows.Forms.Button();
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.buttonBrushReset = new System.Windows.Forms.Button();
            this.buttonBrushBigger = new System.Windows.Forms.Button();
            this.buttonBrushSmaller = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.buttonZoomReset, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonZoomIn, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonZoomOut, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonBrushReset, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonBrushBigger, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonBrushSmaller, 5, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(566, 58);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // buttonZoomReset
            // 
            this.buttonZoomReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonZoomReset.Location = new System.Drawing.Point(3, 3);
            this.buttonZoomReset.Name = "buttonZoomReset";
            this.buttonZoomReset.Size = new System.Drawing.Size(134, 52);
            this.buttonZoomReset.TabIndex = 0;
            this.buttonZoomReset.Text = "Zoom Reset";
            this.buttonZoomReset.UseVisualStyleBackColor = true;
            this.buttonZoomReset.Click += new System.EventHandler(this.buttonZoomReset_Click);
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonZoomIn.Location = new System.Drawing.Point(143, 3);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(49, 52);
            this.buttonZoomIn.TabIndex = 1;
            this.buttonZoomIn.Text = "(+)";
            this.buttonZoomIn.UseVisualStyleBackColor = true;
            this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonZoomOut.Location = new System.Drawing.Point(198, 3);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(49, 52);
            this.buttonZoomOut.TabIndex = 2;
            this.buttonZoomOut.Text = "(-)";
            this.buttonZoomOut.UseVisualStyleBackColor = true;
            this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
            // 
            // buttonBrushReset
            // 
            this.buttonBrushReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonBrushReset.Location = new System.Drawing.Point(253, 3);
            this.buttonBrushReset.Name = "buttonBrushReset";
            this.buttonBrushReset.Size = new System.Drawing.Size(134, 52);
            this.buttonBrushReset.TabIndex = 3;
            this.buttonBrushReset.Text = "Brush Reset";
            this.buttonBrushReset.UseVisualStyleBackColor = true;
            this.buttonBrushReset.Click += new System.EventHandler(this.buttonBrushReset_Click);
            // 
            // buttonBrushBigger
            // 
            this.buttonBrushBigger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonBrushBigger.Location = new System.Drawing.Point(393, 3);
            this.buttonBrushBigger.Name = "buttonBrushBigger";
            this.buttonBrushBigger.Size = new System.Drawing.Size(49, 52);
            this.buttonBrushBigger.TabIndex = 4;
            this.buttonBrushBigger.Text = "(+)";
            this.buttonBrushBigger.UseVisualStyleBackColor = true;
            this.buttonBrushBigger.Click += new System.EventHandler(this.buttonBrushBigger_Click);
            // 
            // buttonBrushSmaller
            // 
            this.buttonBrushSmaller.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonBrushSmaller.Location = new System.Drawing.Point(448, 3);
            this.buttonBrushSmaller.Name = "buttonBrushSmaller";
            this.buttonBrushSmaller.Size = new System.Drawing.Size(49, 52);
            this.buttonBrushSmaller.TabIndex = 5;
            this.buttonBrushSmaller.Text = "(-)";
            this.buttonBrushSmaller.UseVisualStyleBackColor = true;
            this.buttonBrushSmaller.Click += new System.EventHandler(this.buttonBrushSmaller_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1641, 1075);
            this.splitContainer1.SplitterDistance = 58;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1172, 1103);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // MaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1641, 1075);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MaskForm";
            this.Text = "CAVAPA: Edit Frame Mask";
            this.Load += new System.EventHandler(this.MaskForm_Load);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonZoomReset;
        private System.Windows.Forms.Button buttonZoomIn;
        private System.Windows.Forms.Button buttonZoomOut;
        private System.Windows.Forms.Button buttonBrushReset;
        private System.Windows.Forms.Button buttonBrushBigger;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonBrushSmaller;
    }
}