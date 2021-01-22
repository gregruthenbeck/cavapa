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
        public double _valMin = 0.5;
        public double ValMin {
            get {
                return _valMin;
            }
            set {
                _valMin = value;
            }
        }
        public double _valMax = 0.99;
        public double ValMax {
            get {
                return _valMax;
            }
            set {
                _valMax = value;
            }
        }
        private double _val = 0.9;
        public double Val {
            get {
                return _val;
            }
            set {
                if (value < _valMin || value > _valMax)
                    return;

                _val = value;
                textBox1.Text = value.ToString();
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

        private bool initializing = true;

        public TrackBarEx() {
            InitializeComponent();
        }

        private void TrackBarEx_Load(object sender, EventArgs e) {
            textBox1.Text = _val.ToString();
            textBox1_KeyPress(this, null);
            initializing = false;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e) {
            if (initializing)
                return;

            _val = _valMin + ((double)trackBar1.Value / 10.0) * (_valMax - _valMin);
            textBox1.Text = _val.ToString("N2");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            double d = 0.0;
            if (double.TryParse(textBox1.Text, out d)) {
                if (d > ValMax) {
                    trackBar1.Value = 10;
                    _val = ValMax;
                } else if (d > (ValMin - double.Epsilon)) {
                    trackBar1.Value = (int)((d - _valMin) / (_valMax - _valMin) * 10.0);
                    if (!initializing)
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
