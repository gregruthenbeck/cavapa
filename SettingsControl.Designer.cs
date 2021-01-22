
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.trackBarTrailStrength = new System.Windows.Forms.TrackBar();
            this.labelTrailStrength = new System.Windows.Forms.Label();
            this.textBoxTrailStrength = new System.Windows.Forms.TextBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.trackBarEx1 = new cavapa.TrackBarEx();
            this.trackBarEx2 = new cavapa.TrackBarEx();
            this.trackBarEx3 = new cavapa.TrackBarEx();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTrailStrength)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(465, 918);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(120, 63);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(368, 41);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(170, 24);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Shadow Reduction";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // trackBarTrailStrength
            // 
            this.trackBarTrailStrength.Location = new System.Drawing.Point(166, 42);
            this.trackBarTrailStrength.Name = "trackBarTrailStrength";
            this.trackBarTrailStrength.Size = new System.Drawing.Size(308, 69);
            this.trackBarTrailStrength.TabIndex = 2;
            this.trackBarTrailStrength.ValueChanged += new System.EventHandler(this.trackBarTrailStrength_ValueChanged);
            // 
            // labelTrailStrength
            // 
            this.labelTrailStrength.AutoSize = true;
            this.labelTrailStrength.Location = new System.Drawing.Point(15, 48);
            this.labelTrailStrength.Name = "labelTrailStrength";
            this.labelTrailStrength.Size = new System.Drawing.Size(145, 20);
            this.labelTrailStrength.TabIndex = 3;
            this.labelTrailStrength.Text = "Glow-Trail Strength";
            // 
            // textBoxTrailStrength
            // 
            this.textBoxTrailStrength.Location = new System.Drawing.Point(480, 42);
            this.textBoxTrailStrength.Name = "textBoxTrailStrength";
            this.textBoxTrailStrength.Size = new System.Drawing.Size(84, 26);
            this.textBoxTrailStrength.TabIndex = 4;
            this.textBoxTrailStrength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxTrailStrength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTrailStrength_KeyPress);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(339, 918);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(120, 63);
            this.buttonApply.TabIndex = 5;
            this.buttonApply.Text = "&Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackBarTrailStrength);
            this.groupBox1.Controls.Add(this.labelTrailStrength);
            this.groupBox1.Controls.Add(this.textBoxTrailStrength);
            this.groupBox1.Location = new System.Drawing.Point(3, 743);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(582, 114);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Render Settings";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Location = new System.Drawing.Point(3, 435);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(582, 253);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Processing Settings";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.trackBarEx1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.trackBarEx2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.trackBarEx3, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(589, 217);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // trackBarEx1
            // 
            this.trackBarEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarEx1.Label = "The Label";
            this.trackBarEx1.LabelWidth = 180;
            this.trackBarEx1.Location = new System.Drawing.Point(0, 0);
            this.trackBarEx1.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.trackBarEx1.Name = "trackBarEx1";
            this.trackBarEx1.Size = new System.Drawing.Size(577, 40);
            this.trackBarEx1.TabIndex = 0;
            // 
            // trackBarEx2
            // 
            this.trackBarEx2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarEx2.Label = "The Label";
            this.trackBarEx2.LabelWidth = 180;
            this.trackBarEx2.Location = new System.Drawing.Point(0, 40);
            this.trackBarEx2.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.trackBarEx2.Name = "trackBarEx2";
            this.trackBarEx2.Size = new System.Drawing.Size(577, 40);
            this.trackBarEx2.TabIndex = 1;
            // 
            // trackBarEx3
            // 
            this.trackBarEx3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarEx3.Label = "The Label";
            this.trackBarEx3.LabelWidth = 120;
            this.trackBarEx3.Location = new System.Drawing.Point(0, 80);
            this.trackBarEx3.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.trackBarEx3.Name = "trackBarEx3";
            this.trackBarEx3.Size = new System.Drawing.Size(577, 40);
            this.trackBarEx3.TabIndex = 2;
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonClose);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(589, 984);
            this.Load += new System.EventHandler(this.SettingsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTrailStrength)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TrackBar trackBarTrailStrength;
        private System.Windows.Forms.Label labelTrailStrength;
        private System.Windows.Forms.TextBox textBoxTrailStrength;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private TrackBarEx trackBarEx1;
        private TrackBarEx trackBarEx2;
        private TrackBarEx trackBarEx3;
    }
}
