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
    public partial class TrackBarEx : UserControl
    {
        public double ValMin = 0.5;
        public double ValMax = 0.99;
        private double _val = 0.9;
        public double Val {
            get {
                return _val;
            }
        }

        public string Label {
            get {
                return label1.Text;
            }
            set {
                label1.Text = value;
            }
        }

        public int LabelWidth {
            get {
                return (int)tableLayoutPanel1.ColumnStyles[0].Width;
            }
            set {
                tableLayoutPanel1.ColumnStyles[0].Width = value;
            }
        }

        public TrackBarEx() {
            InitializeComponent();
        }

        private void TrackBarEx_Load(object sender, EventArgs e) {
            textBox1.Text = _val.ToString("N1");
            textBox1_KeyPress(this, null);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e) {
            _val = ValMin + ((double)trackBar1.Value / 10.0) * (ValMax - ValMin);
            textBox1.Text = _val.ToString("N2");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            double d = 0.0;
            if (double.TryParse(textBox1.Text, out d)) {
                if (d > ValMax) {
                    trackBar1.Value = 10;
                    _val = ValMax;
                } else if (d > (ValMin - double.Epsilon)) {
                    trackBar1.Value = (int)((d - ValMin) / (ValMax - ValMin) * 10.0);
                    _val = d;
                } else {
                    trackBar1.Value = 0;
                    _val = 0.0;
                }

                textBox1.BackColor = Color.LightGreen;
            } else {
                textBox1.BackColor = Color.Pink;
            }
        }
    }
}
