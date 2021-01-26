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
    public partial class ProcessingCompleteForm : Form
    {
        public bool OpenAfterExportEnabled {
            get {
                return checkBoxOpenInExcel.Checked;
            }
        }

        public string Filename {
            get {
                return labelFilename.Text;
            }
            set {
                labelFilename.Text = value;
            }
        }

        public ProcessingCompleteForm() {
            InitializeComponent();
        }

        private void ProcessingCompleteForm_Load(object sender, EventArgs e) {
            this.buttonOk.Focus();
        }
    }

    public class ProcessingCompletedEventArgs : EventArgs
    {
        private readonly bool _openAfterExport;

        public ProcessingCompletedEventArgs(bool openAfterExportEnabled) {
            _openAfterExport = openAfterExportEnabled;
        }

        public bool OpenAfterExport {
            get { return _openAfterExport; }
        }
    }
}
