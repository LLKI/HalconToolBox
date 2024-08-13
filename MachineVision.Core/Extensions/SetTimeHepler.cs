using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Core.Extensions
{
    public static class SetTimeHepler
    {
        public static double SetTimer(this Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Task.Run(() =>
            {
                action();
            });
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
