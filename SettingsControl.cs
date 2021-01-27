using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        public bool EnableDespeckle {
            get {
                return checkBoxDeSpeckle.Checked;
            }
            set {
                checkBoxDeSpeckle.Checked = value;
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

        public const string _settingsFilename = "cavapa_settings.txt";

        public SettingsControl() {
            InitializeComponent();
        }

        private void SettingsControl_Load(object sender, EventArgs e) {
            LoadSettings();
        }

        public void SaveSettings(string filepath = _settingsFilename) {
            var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            using (TextWriter file = File.CreateText(filepath)) {
                file.WriteLine($"# \"{Path.GetFileName(filepath)}\"");
                file.WriteLine($"# CAVAPA v{ver.ToString()} settings file.");
                file.WriteLine($"# Created " + DateTime.Now.ToString());
                file.WriteLine();
                file.WriteLine($"FrameSmoothingAlpha={FrameSmoothingAlpha.ToString()}");
                file.WriteLine($"EnableShadowReduction={EnableShadowReduction.ToString()}");
                file.WriteLine($"MovementHistoryDecay={MovementHistoryDecay.ToString()}");
                file.WriteLine($"MovementScoreMul={MovementScoreMul.ToString()}");
                file.WriteLine($"MovementPixMul={MovementPixMul.ToString()}");
                file.WriteLine($"MovementNoiseFloor={MovementNoiseFloor.ToString()}");
                file.WriteLine($"EnableDespeckle={EnableDespeckle.ToString()}");
                file.WriteLine();
                file.Flush();
                file.Close();
            }
        }

        public void LoadSettings(string filepath = _settingsFilename) {
            if (!File.Exists(filepath))
                return;

            using (TextReader file = File.OpenText(filepath)) {
                var data = new Dictionary<string, string>();
                String line = "";
                while (true) {
                    line = file.ReadLine();
                    if (line == null)
                        break;
                    line = line.Trim(); // trim whitespace
                    if (line.Length == 0 || line[1] == '#')
                        continue;
                    if (line.Contains('#')) {
                        line = line.Substring(0, line.IndexOf('#')).Trim(); // ignore after the #
                    }
                    string[] splitLine = line.Split(new char[] { '=' });
                    if (splitLine.Length == 2) {
                        data[splitLine[0]] = splitLine[1];
                    }
                }
                file.Close();

                if (data.ContainsKey("FrameSmoothingAlpha")) {
                    double d = 0;
                    if (double.TryParse(data["FrameSmoothingAlpha"], out d))
                        FrameSmoothingAlpha = d;
                }
                if (data.ContainsKey("EnableShadowReduction")) {
                    bool d = false;
                    if (bool.TryParse(data["EnableShadowReduction"], out d))
                        EnableShadowReduction = d;
                }
                if (data.ContainsKey("EnableDespeckle")) {
                    bool d = false;
                    if (bool.TryParse(data["EnableDespeckle"], out d))
                        EnableDespeckle = d;
                }
                if (data.ContainsKey("MovementHistoryDecay")) {
                    double d = 0;
                    if (double.TryParse(data["MovementHistoryDecay"], out d))
                        MovementHistoryDecay = d;
                }
                if (data.ContainsKey("MovementScoreMul")) {
                    double d = 0;
                    if (double.TryParse(data["MovementScoreMul"], out d))
                        MovementScoreMul = d;
                }
                if (data.ContainsKey("MovementPixMul")) {
                    double d = 0;
                    if (double.TryParse(data["MovementPixMul"], out d))
                        MovementPixMul = d;
                }
                if (data.ContainsKey("MovementNoiseFloor")) {
                    double d = 0;
                    if (double.TryParse(data["MovementNoiseFloor"], out d))
                        MovementNoiseFloor = d;
                }
            }
        }
    }
}
