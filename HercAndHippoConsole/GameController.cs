using HercAndHippoLibCs;

namespace HercAndHippoConsole;

public abstract class GameController
{
    public abstract IEnumerable<ActionInput> NextAction(Level state);
}
internal class KeyboardController : GameController
{
    public override IEnumerable<ActionInput> NextAction(Level state)
    {
        ConsoleKeyInfo keyInfo = Console.KeyAvailable ? Console.ReadKey() : default;
        return new ActionInput[] { keyInfo.ToActionInput() };
    }
}

public class EnumerableController : GameController
{
    private readonly IEnumerator<ActionInput> enumerator;
    public EnumerableController(IEnumerable<ActionInput> actions)
    {
        this.enumerator = actions.GetEnumerator();
    }
    public override IEnumerable<ActionInput> NextAction(Level state)
    {
        bool read = enumerator.MoveNext();
        return new ActionInput[] { read ? enumerator.Current : ActionInput.NoAction };
    }
}