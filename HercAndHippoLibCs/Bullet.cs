namespace HercAndHippoLibCs;

public record Bullet(Location Location, Direction Whither) : HercAndHippoObj, ILocatable, ICyclable, IConsoleDisplayable
{
    public string ConsoleDisplayString => "○";
    public Color Color => Color.White;
    public Color BackgroundColor => Color.Black;
    /// <summary>When the level cycles, a bullet moves in the direction it's currently heading.</summary>
    public Level Cycle(Level curState, ActionInput actionInput)
    {
        // Call OnShot methods for any IShootable at current location 
        var shootables = curState
            .ObjectsAt(Location)
            .Where(obj => obj.IsShootable)
            .Cast<IShootable>()
            .ToList();
        Level nextState = shootables
            .Aggregate(seed: curState, func: (state, shot) => shot.OnShot(state, shotFrom: Whither.Mirror(), shotBy: this));

        if (shootables.Any(s => s.StopsBullet))
            nextState = nextState.Without(this);

        // Die if at boundary
        if (ReachedBoundary(nextState.Width, nextState.Height))
            nextState = nextState.Without(this);       

        // Continue moving in current direction if it hasn't been stopped
        nextState = nextState.Contains(this) ?
            nextState.Replace(this, this with { Location = NextLocation }) : // If bullet wasn't stopped, continue
            nextState; // If bullet was stopped, don't regenerate it

        return nextState;
    }

    private Location NextLocation
        => Whither switch
        {
            Direction.North => Location with { Row = Location.Row - 1 },
            Direction.South => Location with { Row = Location.Row + 1 },
            Direction.East => Location with { Col = Location.Col + 1 },
            Direction.West => Location with { Col = Location.Col - 1 },
            Direction.Idle => Location,
            _ => throw new NotImplementedException()
        };

    private bool ReachedBoundary(int levelWidth, int levelHeight)
    {
        bool reachedWestBoundary = Whither == Direction.West && Location.Col <= Column.MIN_COL;
        bool reachedEastBoundary = Whither == Direction.East && Location.Col >= levelWidth;
        bool reachedNorthBoundary = Whither == Direction.North && Location.Row <= Row.MIN_ROW;
        bool reachedSouthBoundary = Whither == Direction.South && Location.Row >= levelHeight;
        return reachedWestBoundary || reachedEastBoundary || reachedNorthBoundary || reachedSouthBoundary;
    }

    public override bool BlocksMotion(Level level) => false;
}
