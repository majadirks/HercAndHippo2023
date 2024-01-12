using HercAndHippoLibCs;

namespace HercAndHippoConsole;

internal record StateAndDiffs(Level State, ScrollStatus ScrollStatus, IEnumerable<DisplayDiff> Diffs);
internal record CacheStats(double CachedCompleted, double CachedIncomplete, double AbInitio)
{
    public override string ToString()
    {
        int cc = (int)Math.Round(CachedCompleted * 100.0);
        int ci = (int)Math.Round(CachedIncomplete * 100.0);
        int ai = (int)Math.Round(AbInitio * 100.0);
        return $"CachedCompleted: {cc}%, CachedIncomplete: {ci}%, AbInitio:{ai}%"; 
    }
}
internal class FutureStates
{
    private readonly DisplayPlan initialPlan;
    private readonly Level initialState;
    private readonly ScrollStatus initialScrollStatus;
    private readonly BufferStats bufferStats;
    private readonly Dictionary<ActionInput, Task<StateAndDiffs>> futures;
    private readonly CancellationTokenSource? cts;

    // Track how often we refresh in different ways
    private enum RefreshType { CachedCompleted, CachedIncomplete, AbInitio }
    private static readonly Dictionary<RefreshType, int> cacheCounts;
    static FutureStates()
    {
        cacheCounts = new()
        {
            {RefreshType.CachedCompleted,0 },
            {RefreshType.CachedIncomplete,0},
            {RefreshType.AbInitio,0 }
        };
    }

    public static CacheStats GetCacheStats()
    {
        int sum = cacheCounts.Values.Sum();
        double cc = cacheCounts[RefreshType.CachedCompleted] * 1.0 / sum;
        double ci = cacheCounts[RefreshType.CachedIncomplete] * 1.0 / sum;
        double ai = cacheCounts[RefreshType.AbInitio] * 1.0 / sum;
        return new CacheStats(cc, ci, ai);
    }

    public StateAndDiffs GetFutureDiffs(ActionInput actionInput)
    {
        // If relevant diffs for this action input have been calculated, return them
        if (futures.TryGetValue(actionInput, out Task<StateAndDiffs>? value))
        {
            if (value.IsCompleted)
            {
                var ret = value.Result;
                cts?.Cancel(); //cancel others
                cacheCounts[RefreshType.CachedCompleted] += 1;
                return ret;
            }
            else // relevant diff calculation started but did not complete, so wait for it to finish
            {
                value.Wait();
                cts?.Cancel();
                cacheCounts[RefreshType.CachedIncomplete] += 1;
                return value.Result;
            }
        }
        else // relevant diff calculation didn't even start! Do it now
        {
            cts?.Cancel();
            cacheCounts[RefreshType.AbInitio] += 1;
            return GetDiffs(
                initialPlan: initialPlan,
                initialState: initialState, 
                actionInput: actionInput, 
                initialScrollStatus: initialScrollStatus, 
                bufferStats: bufferStats);
        }
            
    }
        
    private static readonly ActionInput[] possibleInputs = Enum.GetValues(typeof(ActionInput))
        .Cast<ActionInput>()
        .Where(ai => ai != ActionInput.Quit)
        .ToArray();
    
    /// <summary>
    /// Anticipate possible future states based on all possible inputs, 
    /// if doing so is likely to be "fast" (ie possible to compute all in less than one cycle).
    /// Prioritize computing the state from the most recent input, on the assumption
    /// that a player will often use the same input multiple times consecutively.
    /// </summary>
    /// <param name="initialState">Current game state</param>
    /// <param name="mostRecentInput">Most recent input</param>
    /// <param name="averageCycleTime">Average time to calculate a cycle</param>
    /// <param name="msPerCycle">Ideal interval between cycles</param>
    public FutureStates(DisplayPlan initialPlan, Level initialState, ScrollStatus scrollStatus, BufferStats bufferStats, ActionInput mostRecentInput)
    {
        futures = new();
        cts = null;
        this.initialPlan = initialPlan;
        this.initialState = initialState;
        initialScrollStatus = scrollStatus;
        this.bufferStats = bufferStats;
        

        cts = new();
        Task<StateAndDiffs> fromMostRecent =
            Task.Run(() => GetDiffs(
                initialPlan: initialPlan,
                initialState: initialState, 
                actionInput: mostRecentInput, 
                initialScrollStatus: initialScrollStatus, 
                bufferStats: bufferStats));
        futures.Add(mostRecentInput, fromMostRecent);

        for (int i = 0; i < possibleInputs.Length; i++)
        {
            var actionInput = possibleInputs[i];
            if (actionInput == mostRecentInput) 
                continue;
            else
                futures.Add(actionInput, 
                    Task.Run(() => GetDiffs(
                        initialPlan: initialPlan,
                        initialState: initialState, 
                        actionInput: actionInput, 
                        initialScrollStatus: initialScrollStatus, 
                        bufferStats: bufferStats)));
        }
    }

    private static StateAndDiffs GetDiffs(DisplayPlan initialPlan, Level initialState, ActionInput actionInput, ScrollStatus initialScrollStatus, BufferStats bufferStats)
    {
        Level nextState = initialState.RefreshCyclables(actionInput);
        ScrollStatus nextScrollStatus = initialScrollStatus.Update(nextState.Player.Location, bufferStats);
        DisplayPlan newPlan = new(nextState, nextScrollStatus, bufferStats);
        var diffs = initialPlan.GetDiffs(newPlan);
        return new(State: nextState, ScrollStatus: nextScrollStatus, Diffs: diffs);
    }
}
