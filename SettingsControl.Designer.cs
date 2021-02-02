
namespace cavapa
{
    partial class SettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonClose = new System.Windows.Forms.Button();
            this.checkBoxShadowReduceEnabled = new System.Windows.Forms.CheckBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackBarExGlowTrail = new cavapa.TrackBarEx();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.trackBarExFrameSmoothAlpha = new cavapa.TrackBarEx();
            this.trackBarExNoiseThresh = new cavapa.TrackBarEx();
            this.trackBarExMoveScoreMul = new cavapa.TrackBarEx();
            this.trackBarExMoveMul = new cavapa.TrackBarEx();
            this.checkBoxDeSpeckle = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(466, 367);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(120, 63);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // checkBoxShadowReduceEnabled
            // 
            this.checkBoxShadowReduceEnabled.AutoSize = true;
            this.checkBoxShadowReduceEnabled.Location = new System.Drawing.Point(0, 345);
            this.checkBoxShadowReduceEnabled.Name = "checkBoxShadowReduceEnabled";
            this.checkBoxShadowReduceEnabled.Padding = new System.Windows.Forms.Padding(120, 0, 0, 0);
            this.checkBoxShadowReduceEnabled.Size = new System.Drawing.Size(290, 24);
            this.checkBoxShadowReduceEnabled.TabIndex = 1;
            this.checkBoxShadowReduceEnabled.Text = "Shadow Reduction";
            this.checkBoxShadowReduceEnabled.UseVisualStyleBackColor = true;
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(340, 367);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(120, 63);
            this.buttonApply.TabIndex = 5;
            this.buttonApply.Text = "&Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackBarExGlowTrail);
            this.groupBox1.Location = new System.Drawing.Point(3, 238);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 9);
            this.groupBox1.Size = new System.Drawing.Size(582, 75);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Render Settings";
            // 
            // trackBarExGlowTrail
            // 
            this.trackBarExGlowTrail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarExGlowTrail.Label = "Glow-Trail Strength";
            this.trackBarExGlowTrail.LabelWidth = 160;
            this.trackBarExGlowTrail.Location = new System.Drawing.Point(3, 22);
            this.trackBarExGlowTrail.Margin = new System.Windows.Forms.Padding(3, 3, 3, 8);
            this.trackBarExGlowTrail.Name = "trackBarExGlowTrail";
            this.trackBarExGlowTrail.Size = new System.Drawing.Size(576, 44);
            this.trackBarExGlowTrail.TabIndex = 5;
            this.trackBarExGlowTrail.Val = 0.9D;
            this.trackBarExGlowTrail.ValMax = 0.99D;
            this.trackBarExGlowTrail.ValMin = 0.5D;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.trackBarExFrameSmoothAlpha, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.trackBarExNoiseThresh, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.trackBarExMoveScoreMul, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.trackBarExMoveMul, 0, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(589, 217);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // trackBarExFrameSmoothAlpha
            // 
            this.trackBarExFrameSmoothAlpha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarExFrameSmoothAlpha.Label = "Smoothing Alpha";
            this.trackBarExFrameSmoothAlpha.LabelWidth = 180;
            this.trackBarExFrameSmoothAlpha.Location = new System.Drawing.Point(0, 20);
            this.trackBarExFrameSmoothAlpha.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.trackBarExFrameSmoothAlpha.Name = "trackBarExFrameSmoothAlpha";
            this.trackBarExFrameSmoothAlpha.Size = new System.Drawing.Size(577, 40);
            this.trackBarExFrameSmoothAlpha.TabIndex = 0;
            this.trackBarExFrameSmoothAlpha.Val = 0.9D;
            this.trackBarExFrameSmoothAlpha.ValMax = 0.999D;
            this.trackBarExFrameSmoothAlpha.ValMin = 0.8D;
            // 
            // trackBarExNoiseThresh
            // 
            this.trackBarExNoiseThresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarExNoiseThresh.Label = "Movement Noise Threshold";
            this.trackBarExNoiseThresh.LabelWidth = 180;
            this.trackBarExNoiseThresh.Location = new System.Drawing.Point(0, 60);
            this.trackBarExNoiseThresh.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.trackBarExNoiseThresh.Name = "trackBarExNoiseThresh";
            this.trackBarExNoiseThresh.Size = new System.Drawing.Size(577, 40);
            this.trackBarExNoiseThresh.TabIndex = 1;
            this.trackBarExNoiseThresh.ValMax = 10D;
            this.trackBarExNoiseThresh.ValMin = 0D;
            this.trackBarExNoiseThresh.Val = 0.7D;
            // 
            // trackBarExMoveScoreMul
            // 
            this.trackBarExMoveScoreMul.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarExMoveScoreMul.Label = "Movement Score X";
            this.trackBarExMoveScoreMul.LabelWidth = 180;
            this.trackBarExMoveScoreMul.Location = new System.Drawing.Point(0, 100);
            this.trackBarExMoveScoreMul.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.trackBarExMoveScoreMul.Name = "trackBarExMoveScoreMul";
            this.trackBarExMoveScoreMul.Size = new System.Drawing.Size(577, 40);
            this.trackBarExMoveScoreMul.TabIndex = 2;
            this.trackBarExMoveScoreMul.ValMax = 100000D;
            this.trackBarExMoveScoreMul.ValMin = 1E-07D;
            this.trackBarExMoveScoreMul.Val = 0.001D;
            // 
            // trackBarExMoveMul
            // 
            this.trackBarExMoveMul.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarExMoveMul.Label = "Movement Pix X";
            this.trackBarExMoveMul.LabelWidth = 180;
            this.trackBarExMoveMul.Location = new System.Drawing.Point(0, 140);
            this.trackBarExMoveMul.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.trackBarExMoveMul.Name = "trackBarExMoveMul";
            this.trackBarExMoveMul.Size = new System.Drawing.Size(577, 40);
            this.trackBarExMoveMul.TabIndex = 3;
            this.trackBarExMoveMul.ValMax = 100D;
            this.trackBarExMoveMul.ValMin = 0.1D;
            this.trackBarExMoveMul.Val = 5D;
            // 
            // checkBoxDeSpeckle
            // 
            this.checkBoxDeSpeckle.AutoSize = true;
            this.checkBoxDeSpeckle.Checked = true;
            this.checkBoxDeSpeckle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDeSpeckle.Location = new System.Drawing.Point(0, 375);
            this.checkBoxDeSpeckle.Name = "checkBoxDeSpeckle";
            this.checkBoxDeSpeckle.Padding = new System.Windows.Forms.Padding(120, 0, 0, 0);
            this.checkBoxDeSpeckle.Size = new System.Drawing.Size(303, 24);
            this.checkBoxDeSpeckle.TabIndex = 9;
            this.checkBoxDeSpeckle.Text = "De-speckle/de-flicker";
            this.checkBoxDeSpeckle.UseVisualStyleBackColor = true;
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxDeSpeckle);
            this.Controls.Add(this.checkBoxShadowReduceEnabled);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonClose);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(589, 433);
            this.Load += new System.EventHandler(this.SettingsControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.CheckBox checkBoxShadowReduceEnabled;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private TrackBarEx trackBarExFrameSmoothAlpha;
        private TrackBarEx trackBarExNoiseThresh;
        private TrackBarEx trackBarExMoveScoreMul;
        private TrackBarEx trackBarExGlowTrail;
        private TrackBarEx trackBarExMoveMul;
        private System.Windows.Forms.CheckBox checkBoxDeSpeckle;
    }
}
