
namespace cavapa
{
    partial class ProcessingCompleteForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessingCompleteForm));
            this.checkBoxOpenInExcel = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.pictureBoxTick = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelFilename = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTick)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxOpenInExcel
            // 
            this.checkBoxOpenInExcel.AutoSize = true;
            this.checkBoxOpenInExcel.Checked = true;
            this.checkBoxOpenInExcel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOpenInExcel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBoxOpenInExcel.Location = new System.Drawing.Point(135, 131);
            this.checkBoxOpenInExcel.Name = "checkBoxOpenInExcel";
            this.checkBoxOpenInExcel.Size = new System.Drawing.Size(159, 24);
            this.checkBoxOpenInExcel.TabIndex = 0;
            this.checkBoxOpenInExcel.Text = "Open &after export";
            this.checkBoxOpenInExcel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(216, 198);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(169, 53);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "Exp&ort Data";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(41, 198);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(169, 53);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // pictureBoxTick
            // 
            this.pictureBoxTick.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pictureBoxTick.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxTick.Image")));
            this.pictureBoxTick.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxTick.InitialImage")));
            this.pictureBoxTick.Location = new System.Drawing.Point(92, 55);
            this.pictureBoxTick.Name = "pictureBoxTick";
            this.pictureBoxTick.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxTick.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxTick.TabIndex = 3;
            this.pictureBoxTick.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.Location = new System.Drawing.Point(130, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Processing Complete";
            // 
            // labelFilename
            // 
            this.labelFilename.AutoSize = true;
            this.labelFilename.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelFilename.Location = new System.Drawing.Point(135, 94);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(14, 20);
            this.labelFilename.TabIndex = 5;
            this.labelFilename.Text = "-";
            // 
            // ProcessingCompleteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 263);
            this.Controls.Add(this.labelFilename);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxTick);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkBoxOpenInExcel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcessingCompleteForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "100% Processing Complete";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ProcessingCompleteForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTick)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxOpenInExcel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.PictureBox pictureBoxTick;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelFilename;
    }
}