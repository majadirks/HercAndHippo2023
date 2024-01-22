using System.ComponentModel.Design;

namespace HercAndHippoLibCs;

public record Bullet(Location Location, Direction Whither) : HercAndHippoObj, ILocatable, ITouchable, ICyclable, IConsoleDisplayable
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
        else if (ReachedBoundary(nextState.Width, nextState.Height))
            return nextState.Without(this);
            
        // If direction is Seek or Flee, figure out which way that is and just go straight in that direction
        Direction dir = Whither;
        if (Whither == Direction.Seek)
            dir = this.Seek(nextState, nextState.Player, out int _);
        else if (Whither == Direction.Flee)
            dir = this.Flee(nextState, nextState.Player);
            
        // Continue moving in current direction if it hasn't been stopped
        nextState = nextState.Contains(this) ?
            nextState.ReplaceIfPresent(this, this with { Whither = dir, Location = NextLocation(Location, dir) }) :
            nextState; // If bullet was stopped, don't regenerate it

        return nextState;
    }

    private static Location NextLocation(Location startLoc, Direction dir)
    {
        return dir switch
        {
            Direction.North => startLoc with { Row = startLoc.Row - 1 },
            Direction.South => startLoc with { Row = startLoc.Row + 1 },
            Direction.East => startLoc with { Col = startLoc.Col + 1 },
            Direction.West => startLoc with { Col = startLoc.Col - 1 },
            Direction.Idle => startLoc,
            _ => throw new NotImplementedException()
        };
    }

    private bool ReachedBoundary(int levelWidth, int levelHeight)
    {
        bool reachedWestBoundary = Whither == Direction.West && Location.Col <= Column.MIN_COL;
        bool reachedEastBoundary = Whither == Direction.East && Location.Col >= levelWidth;
        bool reachedNorthBoundary = Whither == Direction.North && Location.Row <= Row.MIN_ROW;
        bool reachedSouthBoundary = Whither == Direction.South && Location.Row >= levelHeight;
        return reachedWestBoundary || reachedEastBoundary || reachedNorthBoundary || reachedSouthBoundary;
    }

    public override bool BlocksMotion(Level level, ILocatable toBlock) => false;

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        if (touchedBy is IShootable toShoot)
            return toShoot.OnShot(level, Direction.Idle, this).Without(this);
        else
            return Behaviors.NoReaction(level);
    }
}
