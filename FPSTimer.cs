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

        public FPSTimer(int _interval = 25) 
        {
            this.interval = _interval;
            timer = new Stopwatch();
            timer.Start();
        }

        public int Update() 
        {
            if (++count == interval)
            {
                fps = (int)(1.08 * 1000.0 / (double)(timer.ElapsedMilliseconds / (double)(interval)) + 0.5);
                count = 0;
                timer.Restart();
            }

            return fps;
        }
    }
}
