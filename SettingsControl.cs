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
    public partial class SettingsControl : UserControl {
        public double FrameSmoothingAlpha {
            get {
                return trackBarExFrameSmoothAlpha.Val;
            }
            set {
                trackBarExFrameSmoothAlpha.Val = value;
            }
        }
        public bool EnableShadowReduction {
            get {
                return checkBoxShadowReduceEnabled.Checked;
            }
            set {
                checkBoxShadowReduceEnabled.Checked = value;
            }
        }
        public double MovementHistoryDecay {
            get {
                return trackBarExGlowTrail.Val;
            }
            set {
                trackBarExGlowTrail.Val = value;
            }
        }
        public double MovementScoreMul {
            get {
                return trackBarExMoveScoreMul.Val;
            }
            set {
                trackBarExMoveScoreMul.Val = value;
            }
        }
        public double MovementPixMul {
            get {
                return trackBarExMoveMul.Val;
            }
            set {
                trackBarExMoveMul.Val = value;
            }
        }
        public double MovementNoiseFloor {
            get {
                return trackBarExNoiseThresh.Val;
            }
            set {
                trackBarExNoiseThresh.Val = value;
            }
        }
        public Button AcceptButton {
            get {
                return this.buttonApply; // ! not really
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
        }
    }
}
