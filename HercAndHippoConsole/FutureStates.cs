using HercAndHippoLibCs;

namespace HercAndHippoConsole;

internal record StateAndPlan(Level State, ScrollStatus ScrollStatus, DisplayPlan Plan);
internal class FutureStates
{
    private readonly Level initialState;
    private readonly ScrollStatus initialScrollStatus;
    private readonly BufferStats bufferStats;
    private readonly Dictionary<ActionInput, Task<StateAndPlan>> futures;
    private readonly CancellationTokenSource? cts;
    public StateAndPlan GetFuturePlan(ActionInput actionInput)
    {
        // If next state has been calculated, return it
        if (futures.TryGetValue(actionInput, out Task<StateAndPlan>? value) && value.IsCompleted)
        {
            var ret = value.Result;
            cts?.Cancel(); //cancel others
            return ret;
        }
        else // next state has not been fully calculated.
        {
            cts?.Cancel();
            return GetDisplayPlan(initialState, actionInput, initialScrollStatus, bufferStats);
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
    public FutureStates(Level initialState, ScrollStatus scrollStatus, BufferStats bufferStats, ActionInput mostRecentInput, double averageCycleTime, double msPerCycle)
    {
        futures = new();
        cts = null;
        this.initialState = initialState;
        initialScrollStatus = scrollStatus;
        this.bufferStats = bufferStats;
        bool parallelEnabled = averageCycleTime * possibleInputs.Length < msPerCycle;
        parallelEnabled = false; // debug
        if (!parallelEnabled) return; // Doing all the calculations takes too long to be worthwhile

        cts = new();
        Task<StateAndPlan> fromMostRecent =
            Task.Run(() => GetDisplayPlan(initialState, mostRecentInput, initialScrollStatus, bufferStats));
        futures.Add(mostRecentInput, fromMostRecent);

        for (int i = 0; i < possibleInputs.Length; i++)
        {
            var actionInput = possibleInputs[i];
            if (actionInput == mostRecentInput) 
                continue;
            else
                futures.Add(actionInput, 
                    Task.Run(() => GetDisplayPlan(initialState, actionInput, initialScrollStatus, bufferStats)));
        }
    }

    private static StateAndPlan GetDisplayPlan(Level initialState, ActionInput actionInput, ScrollStatus initialScrollStatus, BufferStats bufferStats)
    {
        Level nextState = initialState.RefreshCyclables(actionInput);
        ScrollStatus nextScrollStatus = initialScrollStatus.Update(nextState.Player.Location, bufferStats);
        return new(State: nextState, ScrollStatus: nextScrollStatus, Plan: new DisplayPlan(nextState, nextScrollStatus, bufferStats));
    }
}
