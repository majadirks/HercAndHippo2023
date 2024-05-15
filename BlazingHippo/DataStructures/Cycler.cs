using System.Diagnostics;
namespace BlazingHippo;
internal class Cycler
{
    private readonly Stopwatch sw;
    public int MillisecondsPerCycle { get; init; }
    private readonly IProgress<bool> progress;
    private readonly CancellationToken token;
    public Cycler(int frequencyHz, IProgress<bool> progress, CancellationToken token)
    {
        sw = Stopwatch.StartNew();
        MillisecondsPerCycle = 1000 / frequencyHz;
        this.progress = progress;
        this.token = token;
        Task.Run(Cycle, token);
    }

    private bool Cycled()
    {
        if (sw.ElapsedMilliseconds > MillisecondsPerCycle)
        {
            sw.Restart();
            progress.Report(true);
            return true;
        }
        return false;
    }

    private void Cycle()
    {
        while (true)
        {
            while (!Cycled())
            {
            }
            token.ThrowIfCancellationRequested();
        }
    }
}
