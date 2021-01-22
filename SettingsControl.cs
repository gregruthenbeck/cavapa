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
    public partial class SettingsControl : UserControl
    {
        public int backgroundFrameBlendInterval {
            get {
                return 25;
            }
        }
        public int backgroundFrameBlendCount {
            get {
                return 10;
            }
        }
        public int frameBlendCount {
            get {
                return 3;
            }
        }
        public bool enableShadowReduction {
            get {
                return false;
            }
        }
        public double movementNoiseFloor {
            get {
                return 0.7;
            }
        }
        public double movementMultiplier {
            get {
                return 5.0;
            }
        }
        private double trailMin = 0.5; // min movementHistoryDecay
        private double trailMax = 0.99; // max movementHistoryDecay
        private double _movementHistoryDecay = 0.9;
        public double movementHistoryDecay {
            get {
                return _movementHistoryDecay;
            }
        }
        public double movementScoreMul {
            get {
                return 1E-3; // 1E-3
            }
        }
        public Button CloseButton {
            get {
                return this.buttonClose;
            }
        }


        public SettingsControl() {
            InitializeComponent();
        }

        private void SettingsControl_Load(object sender, EventArgs e) {
            textBoxTrailStrength.Text = _movementHistoryDecay.ToString("N1");
            textBoxTrailStrength_KeyPress(this, null);
        }

        private void trackBarTrailStrength_ValueChanged(object sender, EventArgs e) {
            _movementHistoryDecay = trailMin + ((double)trackBarTrailStrength.Value / 10.0) * (trailMax - trailMin);
            textBoxTrailStrength.Text = _movementHistoryDecay.ToString("N2");
        }

        private void textBoxTrailStrength_KeyPress(object sender, KeyPressEventArgs e) {
            double d = 0.0;
            if (double.TryParse(textBoxTrailStrength.Text, out d)) {
                if (d > trailMax) {
                    trackBarTrailStrength.Value = 10;
                    _movementHistoryDecay = trailMax; // max value is 0.99
                } else if (d > (trailMin - double.Epsilon)) {
                    trackBarTrailStrength.Value = (int)((d - trailMin) / (trailMax - trailMin) * 10.0);
                    _movementHistoryDecay = d;
                } else {
                    trackBarTrailStrength.Value = 0;
                    _movementHistoryDecay = 0.0;
                }

                textBoxTrailStrength.BackColor = Color.LightGreen;
            } else {
                textBoxTrailStrength.BackColor = Color.Pink;
            }
        }
    }
}
