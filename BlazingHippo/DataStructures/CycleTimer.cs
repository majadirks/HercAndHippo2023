using System.Diagnostics;
namespace BlazingHippo;
internal class CycleTimer
{
    private readonly Stopwatch sw;
    public int MillisecondsPerCycle { get; init; }
    public CycleTimer(int frequencyHz)
    {
        sw = Stopwatch.StartNew();
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
        while (!Cycled()) { }
    }
}
