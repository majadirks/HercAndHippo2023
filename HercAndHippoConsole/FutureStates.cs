using HercAndHippoLibCs;

namespace HercAndHippoConsole;

internal record StateAndDiffs(Level State, ScrollStatus ScrollStatus, IEnumerable<DisplayDiff> Diffs);
internal class FutureStates
{
    private readonly DisplayPlan initialPlan;
    private readonly Level initialState;
    private readonly ScrollStatus initialScrollStatus;
    private readonly BufferStats bufferStats;
    private readonly Dictionary<ActionInput, Task<StateAndDiffs>> futures;
    private readonly CancellationTokenSource? cts;
    public StateAndDiffs GetFutureDiffs(ActionInput actionInput)
    {
        // If next state has been calculated, return it
        if (futures.TryGetValue(actionInput, out Task<StateAndDiffs>? value) && value.IsCompleted)
        {
            var ret = value.Result;
            cts?.Cancel(); //cancel others
            return ret;
        }
        else // next state has not been fully calculated.
        {
            cts?.Cancel();
            return GetDiffs(
                initialPlan: initialPlan,
                initialState: initialState, 
                actionInput: actionInput, 
                initialScrollStatus: initialScrollStatus, 
                bufferStats: bufferStats);
        }
            
    }
        
    private static readonly ActionInput[] possibleInputs = (ActionInput[])Enum.GetValues(typeof(ActionInput));
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
    public FutureStates(DisplayPlan initialPlan, Level initialState, ScrollStatus scrollStatus, BufferStats bufferStats, ActionInput mostRecentInput, double averageCycleTime, double msPerCycle)
    {
        futures = new();
        cts = null;
        this.initialPlan = initialPlan;
        this.initialState = initialState;
        initialScrollStatus = scrollStatus;
        this.bufferStats = bufferStats;
        bool parallelEnabled = averageCycleTime * possibleInputs.Length < msPerCycle;
        //parallelEnabled = false; // debug
        if (!parallelEnabled) return; // Doing all the calculations takes too long to be worthwhile

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
