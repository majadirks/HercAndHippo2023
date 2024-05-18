using HercAndHippoLibCs;
namespace BlazingHippo;

public class DoNothingController : GameController
{
    public override ActionInputPair NextAction(Level state)
    {
        return ActionInput.NoAction;
    }
}

public class WasdController : GameController
{
    public const int NO_ACTION_KEY = -1;
    private readonly Func<int> getKey;
    public WasdController(Func<int> getKey)
    {
        this.getKey = getKey;
    }

    public static ActionInputPair ActionFromKey(int key)
    {
        ActionInputPair actionInput = key switch
        {
            65 => ActionInput.MoveWest, // pressed "a"
            68 => ActionInput.MoveEast, // "pressed d"
            87 => ActionInput.MoveNorth, // "pressed w"
            83 => ActionInput.MoveSouth, // pressed "s"
            81 => ActionInput.Quit, // pressed "q"
            NO_ACTION_KEY => ActionInput.NoAction,
            _ => ActionInput.NoAction
        };
        return actionInput;
    }

    public override ActionInputPair NextAction(Level state)
    {
        int key = getKey();
        var action = ActionFromKey(key);
        return action;
    }
}

