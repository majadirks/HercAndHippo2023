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
    public const int SHIFT_KEY = 16;
    public const int W = 87;
    public const int A = 65;
    public const int S = 83;
    public const int D = 68;
    public const int Q = 81;
    private readonly Func<HashSet<int>> getKeys;
    public WasdController(Func<HashSet<int>> getKey)
    {
        this.getKeys = getKey;
    }

    public static ActionInputPair ActionFromKeys(HashSet<int> keys)
    {
        bool modified = keys.Contains(SHIFT_KEY);
        
        if (modified)
        {
            if (keys.Contains(A))
                return ActionInput.ShootWest;
            else if (keys.Contains(D))
                return ActionInput.ShootEast;
            else if (keys.Contains(W))
                return ActionInput.ShootNorth;
            else if (keys.Contains(S))
                return ActionInput.ShootSouth;
            else
                return ActionInput.NoAction;
        }
        else // no shift key
        {
            if (keys.Contains(A))
                return ActionInput.MoveWest;
            else if (keys.Contains(D))
                return ActionInput.MoveEast;
            else if (keys.Contains(W))
                return ActionInput.MoveNorth;
            else if (keys.Contains(S))
                return ActionInput.MoveSouth;
            else if (keys.Contains(Q))
                return ActionInput.Quit;
            else
                return ActionInput.NoAction;
        }
    }

    public override ActionInputPair NextAction(Level state)
    {
        var keys = getKeys();
        if (!keys.Any())
            return ActionInput.NoAction;
        var action = ActionFromKeys(keys);
        return action;
    }
}

