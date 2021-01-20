using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cavapa
{
    class FPSTimer
    {
        Stopwatch timer;
        int interval = 20;
        int count = 0;
        int fps = 20;

        public FPSTimer() 
        {
            timer = new Stopwatch();
            timer.Start();
        }

        public int Update() 
        {
            if (++count == interval)
            {
                fps = (int)(1000.0 / (double)(timer.ElapsedMilliseconds / (long)(interval)));
                count = 0;
                timer.Restart();
            }

            return fps;
        }
    }
}
