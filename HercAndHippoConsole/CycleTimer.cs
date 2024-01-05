using System.Diagnostics;

namespace HercAndHippoConsole
{
    internal class CycleTimer
    {
        private readonly Stopwatch sw;
        public int MillisecondsPerCycle { get; init; }
        private int halfwayStart;
        private int halfwayEnd;
        public CycleTimer(int frequencyHz)
        {
            halfwayStart = MillisecondsPerCycle / 2;
            halfwayEnd = MillisecondsPerCycle / 1000 + halfwayStart;
            sw = new Stopwatch();
            sw.Start();
            MillisecondsPerCycle = 1000 / frequencyHz;
        }

        private bool Cycled()
        {
            if (sw.ElapsedMilliseconds > MillisecondsPerCycle)
            {
                sw.Restart();
                return true;
            }
            return false;
        }

        public ConsoleKeyInfo AwaitCycle()
        {
            while (!Cycled()) {}
            return Console.KeyAvailable ? Console.ReadKey() : default; 
        }
    }
}
