using HercAndHippoLibCs;
using System.Diagnostics;

namespace HercAndHippoConsole;

public class AverageCycleTimer
{
    private readonly long[] Counts;
    private readonly Stopwatch sw;
    private readonly int iterationCount;
    public AverageCycleTimer(int iterationCount)
    {
        this.iterationCount = iterationCount;
        this.Counts = new long[iterationCount];
        sw = Stopwatch.StartNew();
    }

    public void StartTick() => sw.Restart();
    public double EndTick(Level level)
    {
        long tickLength = sw.ElapsedMilliseconds;
        int index = level.Cycles % iterationCount;
        Counts[index] = tickLength;
        sw.Restart();
        return Counts.Where(ct => ct > 0).Average();
    }

    public double InitialEstimate(Level level)
    {
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < iterationCount; i++)
            level = level.RefreshCyclables(ActionInput.NoAction);
        sw.Stop();
        long totalTime = sw.ElapsedMilliseconds;
        double estimate = totalTime * 1.0 / iterationCount;
        return estimate;
    }
}
