using HercAndHippoLibCs;

namespace HercAndHippoConsole;

internal class FutureStates
{
    private readonly Dictionary<ActionInput, Level> futures;
    public Level this[ActionInput actionInput] => futures[actionInput];
    private static readonly ActionInput[] possibleInputs = (ActionInput[])Enum.GetValues(typeof(ActionInput));
    public FutureStates(Level state)
    {       
        var tuples = possibleInputs
            .AsParallel()
            .Select(actionInput => (actionInput, state.RefreshCyclables(actionInput)));
        futures = new();
        foreach (var tuple in tuples)
        {
            futures.Add(key: tuple.actionInput, value: tuple.Item2);
        }
    }
}
