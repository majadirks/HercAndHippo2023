using HercAndHippoLibCs;

namespace HercAndHippoConsole;

public abstract class GameController
{
    public abstract ActionInput NextAction(Level state);
}
internal class KeyboardController : GameController
{
    public override ActionInput NextAction(Level state)
    {
        ConsoleKeyInfo keyInfo = Console.KeyAvailable ? Console.ReadKey() : default;
        return keyInfo.ToActionInput();
    }
}

public class EnumerableController : GameController
{
    private readonly IEnumerator<ActionInput> enumerator;
    public EnumerableController(IEnumerable<ActionInput> actions)
    {
        this.enumerator = actions.GetEnumerator();
    }
    public override ActionInput NextAction(Level state)
    {
        bool read = enumerator.MoveNext();
        return read ? enumerator.Current : ActionInput.NoAction;
    }
}