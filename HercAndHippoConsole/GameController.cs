using HercAndHippoLibCs;

namespace HercAndHippoConsole;

public abstract class GameController
{
    public abstract ActionInputPair NextAction(Level state);
}
internal class KeyboardController : GameController
{
    public override ActionInputPair NextAction(Level state)
    {
        ConsoleKeyInfo keyInfo = Console.KeyAvailable ? Console.ReadKey() : default;
        return new(keyInfo.ToActionInput());
    }
}

public class EnumerableController : GameController
{
    private readonly IEnumerator<ActionInput> enumerator;
    public EnumerableController(IEnumerable<ActionInput> actions)
    {
        this.enumerator = actions.GetEnumerator();
    }
    public override ActionInputPair NextAction(Level state)
    {
        bool read = enumerator.MoveNext();
        return new ActionInputPair(read ? enumerator.Current: ActionInput.NoAction);
    }
}