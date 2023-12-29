using HercAndHippoLibCs;

namespace HercAndHippoConsole;

internal class FutureStates
{
    private readonly Level initialState;
    private readonly Dictionary<ActionInput, Task<Level>> futures;
    private readonly CancellationTokenSource cts;
    public Level GetState(ActionInput actionInput)
    {
        cts.Cancel();
        if (futures.TryGetValue(actionInput, out Task<Level>? value) && value.IsCompleted)
            return value.Result;
        else
            return initialState.RefreshCyclables(actionInput);  
    }
        
    private static readonly ActionInput[] possibleInputs = (ActionInput[])Enum.GetValues(typeof(ActionInput));
    public FutureStates(Level state, ActionInput mostRecent)
    {
        futures = new();
        cts = new();
        initialState = state;
        Task<Level> fromMostRecent = Task.Run(() => state.RefreshCyclables(mostRecent));
        futures.Add(mostRecent, fromMostRecent);
        
        for (int i = 0; i < possibleInputs.Length; i++)
        {
            var actionInput = possibleInputs[i];
            futures.TryAdd(actionInput, Task.Run(() => state.RefreshCyclables(actionInput, cts.Token)));
        }        
    }
}
