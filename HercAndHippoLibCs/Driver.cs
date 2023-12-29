namespace HercAndHippoLibCs;

public record Driver(Direction Whither, int Modulus, int Count = 0) : HercAndHippoObj, ICyclable
{
    public override bool BlocksMotion(Player p) => false;

    public Level Cycle(Level level, ActionInput actionInput)
    {
        int nextCount = Count + 1;
        int nextMod = Modulus;
        Level nextState = level;

        if (actionInput == ActionInput.MoveNorth)
        {
            nextMod = Math.Max(1, nextMod - 1);
            nextState = nextState.Player.Cycle(nextState, ActionInput.MoveSouth);
        }
        else if (actionInput == ActionInput.MoveSouth)
        {
            nextMod = Math.Min(10, nextMod + 1);
            nextState = nextState.Player.Cycle(nextState, ActionInput.MoveNorth);
        }

        if (Count % Modulus > 0)
            nextState = nextState.Replace(this, this with { Count = nextCount, Modulus = nextMod} );

        ActionInput motion = Whither switch
        {
            Direction.East => ActionInput.MoveEast,
            Direction.West => ActionInput.MoveWest,
            Direction.North => ActionInput.MoveNorth,
            Direction.South => ActionInput.MoveSouth,
            _ => ActionInput.NoAction

        };

        return nextState.Player.Cycle(level, motion).Replace(this, this with { Count = nextCount, Modulus = nextMod });
    }
}
