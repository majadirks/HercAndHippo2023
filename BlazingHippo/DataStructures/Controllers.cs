using HercAndHippoLibCs;
namespace BlazingHippo;

public class DoNothingController : GameController
{
    public override ActionInputPair NextAction(Level state)
    {
        return ActionInput.NoAction;
    }
}

public class SimpleController : GameController
{
    private readonly Func<ActionInput> getAction;
    public SimpleController(Func<ActionInput> getAction)
    {
        this.getAction = getAction;
    }

    public override ActionInputPair NextAction(Level state)
    {
        return getAction();
    }
}

