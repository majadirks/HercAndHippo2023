using HercAndHippoLibCs;
using System.Collections;
namespace HercAndHippoConsole;

internal class ActionInputSet : IEquatable<ActionInputSet>, IEnumerable<ActionInput>
{
    private readonly ActionInput[] inputs;
    public ActionInputSet(ActionInput first, ActionInput? secondOrNull = null)
    {
        ActionInput second = secondOrNull ?? ActionInput.NoAction;
        if (second == first) second = ActionInput.NoAction;
        // Put these in some deterministic order
        bool firstFirst = (int)first < (int)second;
        inputs = firstFirst ? 
            new ActionInput[] { first, second } : 
            new ActionInput[] { second, first };
    }
    public override int GetHashCode() => 19 * (int)inputs[0] + 31 * (int)inputs[1];
    public bool Equals(ActionInputSet? other) 
        => other != null && inputs[0] == other.inputs[0] && inputs[1] == other.inputs[1];
    public override bool Equals(object? obj)
     => obj != null && Equals(obj as ActionInputSet);

    public IEnumerator<ActionInput> GetEnumerator()
        => ((IEnumerable<ActionInput>)inputs).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => inputs.GetEnumerator();
    public ActionInput First => inputs[0];
    public ActionInput Second => inputs[1];
    public override string ToString()
    {
        if (First == ActionInput.NoAction && Second == ActionInput.NoAction)
            return "No action.";
        else if (First == ActionInput.NoAction)
            return Second.ToString();
        else if (Second == ActionInput.NoAction)
            return First.ToString();
        else
            return ($"{First} and {Second}");
    }
}
