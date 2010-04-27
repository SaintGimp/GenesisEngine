using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    static class StopwatchExtensions
    {
        public static long Measure(this Stopwatch stopwatch, Action action)
        {
            stopwatch.Reset();
            stopwatch.Start();

            action();
            
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}
