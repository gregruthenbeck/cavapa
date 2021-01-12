using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cavapa
{
    class ProcessSettings
    {
        public int backgroundFrameBlendInterval = 25;
        public int backgroundFrameBlendCount = 10;
        public int frameBlendCount = 4;
        public bool enableShadowReduction = false;
        public double movementNoiseFloor = 0.7;
        public double movementMultiplier = 5.0;
        public double movementHistoryDecay = 0.9; // 0.9
        public double movementScoreMul = 1E-3; // 1E-3
    }
}
