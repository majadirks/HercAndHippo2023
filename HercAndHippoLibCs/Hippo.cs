namespace HercAndHippoLibCs;

public record Hippo(Location Location, Health Health, bool LockedToPlayer) : HercAndHippoObj, ILocatable, ITouchable, ICyclable, IShootable, IConsoleDisplayable
{
    public Color Color => Color.Magenta;

    public Color BackgroundColor => Color.DarkBlue;

    public string ConsoleDisplayString => "🦛";

    public bool StopsBullet => true;

    public override bool BlocksMotion(Level level)
    {
        // this is gross, and blocks only if moving north
        Level ifPlayerWereNorthOne = level.WithPlayer(level.Player with { Location = this.Location });
        return ifPlayerWereNorthOne.Player.MotionBlockedTo(ifPlayerWereNorthOne, Direction.North);
    }

    public Level Cycle(Level level, ActionInput actionInput)
    {
        if (!Health.HasHealth)
            return Behaviors.Die(level, this);
        else if (LockedToPlayer && actionInput == ActionInput.DropHippo)
            return PutDown(level);
        else if (LockedToPlayer)
            return LockAbovePlayer(level);
        else
            return Behaviors.NoReaction(level);
    }

    private Level LockAbovePlayer(Level level)
    {
        Player player = level.Player;
        Location nextLocation = new(Col: player.Location.Col, Row: player.Location.Row.NextNorth());
        return level.Replace(this, this with { Location = nextLocation, LockedToPlayer = true });
    }

    private Level PutDown(Level level)
    {
        Player player = level.Player;
        // First, attempt to place East
        bool blockedEast = player.ObjectLocatedTo(level, Direction.East) || player.MotionBlockedTo(level,Direction.East);
        if (!blockedEast)
        {
            Location nextLocation = new(Col: player.Location.Col.NextEast(level.Width), Row: player.Location.Row);
            return level.Replace(this, this with { Location = nextLocation, LockedToPlayer = false });
        }

        // If that didn't work, attempt to place West
        bool blockedWest = player.ObjectLocatedTo(level, Direction.West) || player.MotionBlockedTo(level, Direction.West);
        if (!blockedWest)
        {
            Location nextLocation = new(Col: player.Location.Col.NextWest(), Row: player.Location.Row);
            return level.Replace(this, this with { Location = nextLocation, LockedToPlayer = false });
        }

        // Could not put down hippo
        return Behaviors.NoReaction(level);
    }

    public Level OnShot(Level level, Direction shotFrom, Bullet shotBy)
        => level.Without(shotBy).Replace(this, this with { Health = this.Health - 5 });

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        Player player = level.Player;
        bool cannotLift = player.ObjectLocatedTo(level, Direction.North) || player.MotionBlockedTo(level, Direction.North);
        if (cannotLift)
            return Behaviors.NoReaction(level);
        else
            return LockAbovePlayer(level);     
    }
}

