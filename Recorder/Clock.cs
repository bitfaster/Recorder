
using System.Diagnostics;

namespace Recorder
{
    public class Clock
    {
        private static readonly double stopwatchAdjustmentFactor = Stopwatch.Frequency / (double)TimeSpan.TicksPerSecond;

        private readonly long start;
        double lastResult = -1;

        public Clock(long start)
        {
            this.start = start;
        }

        public double Time(long ticks)
        {
            double result = TimeSpan.FromTicks((long)((ticks - this.start) / stopwatchAdjustmentFactor)).TotalMilliseconds;

            // guarantee monotonically increasing time values
            if (result < lastResult)
            {
                return lastResult;
            }

            lastResult = result;

            return result;
        }

        // https://www.speedscope.app/file-format-schema.json
        //"bytes",
        //"microseconds",
        //"milliseconds",
        //"nanoseconds",
        //"none",
        //"seconds"
        public static string Unit => "milliseconds";
    }
}
