using System.Diagnostics;

namespace HercAndHippoConsole
{
    internal class CycleTimer
    {
        private readonly int frequencyHz;
        private readonly Stopwatch sw;
        public CycleTimer(int frequencyHz)
        {
            sw = new Stopwatch();
            sw.Start();
            this.frequencyHz = frequencyHz;
        }

        private bool Cycled()
        {
            if (sw.ElapsedMilliseconds > 1000 / frequencyHz)
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
