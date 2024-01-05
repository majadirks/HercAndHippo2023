using HercAndHippoLibCs;

namespace HercAndHippoConsole;

internal class FutureStates
{
    private readonly Level initialState;
    private readonly Dictionary<ActionInput, Task<Level>> futures;
    private readonly CancellationTokenSource? cts;
    public Level GetState(ActionInput actionInput)
    {
        // If next state has been calculated, return it
        if (futures.TryGetValue(actionInput, out Task<Level>? value) && value.IsCompleted)
        {
            var ret = value.Result;
            cts?.Cancel(); //cancel others
            return ret;
        }
        else // next state has not been fully calculated.
        {
            cts?.Cancel();
            return initialState.RefreshCyclables(actionInput);
        }
            
    }
        
    private static readonly ActionInput[] possibleInputs = (ActionInput[])Enum.GetValues(typeof(ActionInput));
    /// <summary>
    /// Anticipate possible future states based on all possible inputs, 
    /// if doing so is likely to be "fast" (ie possible to compute all in less than one cycle).
    /// Prioritize computing the state from the most recent input, on the assumption
    /// that a player will often use the same input multiple times consecutively.
    /// </summary>
    /// <param name="state">Current game state</param>
    /// <param name="mostRecent">Most recent input</param>
    /// <param name="averageCycleTime">Average time to calculate a cycle</param>
    /// <param name="msPerCycle">Ideal interval between cycles</param>
    public FutureStates(Level state, ActionInput mostRecent, double averageCycleTime, double msPerCycle)
    {
        futures = new();
        cts = null;
        initialState = state;
        bool parallelEnabled = averageCycleTime * possibleInputs.Length < msPerCycle;
        //parallelEnabled = false; // debug
        if (!parallelEnabled) return; // Doing all the calculations takes too long to be worthwhile

        cts = new();
        Task<Level> fromMostRecent = Task.Run(() => state.RefreshCyclables(mostRecent));
        futures.Add(mostRecent, fromMostRecent);
        for (int i = 0; i < possibleInputs.Length; i++)
        {
            var actionInput = possibleInputs[i];
            futures.TryAdd(actionInput, Task.Run(() => state.RefreshCyclables(actionInput, cts.Token)));
        }
    }
}
