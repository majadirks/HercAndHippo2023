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
    public const int X = 88;
    public const int UP = 38;
    public const int RIGHT = 39;
    public const int DOWN = 40;
    public const int LEFT = 37;
    public const int SPACE = 32;
    private readonly Func<HashSet<int>> getKeys;
    public WasdController(Func<HashSet<int>> getKeys)
    {
        this.getKeys = getKeys;
    }

    public static ActionInputPair ActionFromKeys(HashSet<int> keys)
    {
        bool shooting = keys.Contains(SHIFT_KEY);
        bool north = keys.Contains(W) || keys.Contains(UP);
        bool south = keys.Contains(S) || keys.Contains(DOWN);
        bool east = keys.Contains(D) || keys.Contains(RIGHT);
        bool west = keys.Contains(A) || keys.Contains(LEFT);
        bool droppingHippo = keys.Contains(X) || keys.Contains(SPACE);
        bool quitting = keys.Contains(Q);

        if (shooting) // cannot shoot while doing something else
        {
            if (west)
                return ActionInput.ShootWest;
            else if (east)
                return ActionInput.ShootEast;
            else if (north)
                return ActionInput.ShootNorth;
            else if (south)
                return ActionInput.ShootSouth;
            else
                return ActionInput.NoAction;
        }
        if (north) // jumping
        {
            if (east)
                return new(ActionInput.MoveEast, ActionInput.MoveNorth);
            else if (west)
                return new(ActionInput.MoveWest, ActionInput.MoveNorth);
            else // jumping, no horizontal motion
                return ActionInput.MoveNorth;
        }

        if (south)
        {
            if (east)
                return new(ActionInput.MoveEast, ActionInput.MoveSouth);
            else if (west)
                return new(ActionInput.MoveWest, ActionInput.MoveSouth);
            else
                return ActionInput.MoveSouth;
        }
        if (east)
            return ActionInput.MoveEast;
        else if (west)
            return ActionInput.MoveWest;
        else if (south)
            return ActionInput.MoveSouth;
        else if (droppingHippo)
            return ActionInput.DropHippo;
        else if (quitting)
            return ActionInput.Quit;
        else
            return ActionInput.NoAction;
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

