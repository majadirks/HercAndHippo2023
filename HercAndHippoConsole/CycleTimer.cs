using System.Diagnostics;

namespace HercAndHippoConsole
{
    internal class CycleTimer
    {
        private readonly int frequencyHz;
        private readonly Stopwatch sw;
        public int MillisecondsPerCycle { get; init; }
        public CycleTimer(int frequencyHz)
        {
            sw = new Stopwatch();
            sw.Start();
            this.frequencyHz = frequencyHz;
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

        public void AwaitCycle()
        {
            while (!Cycled());
        }
    }
}
