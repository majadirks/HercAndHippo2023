namespace HercAndHippoLibCs;

public record Hippo(Location Location, Health Health, bool LockedToPlayer) : HercAndHippoObj, ILocatable, ITouchable, ICyclable, IShootable, IConsoleDisplayable
{
    public Color Color => Color.Magenta;

    public Color BackgroundColor => Color.DarkBlue;

    public string ConsoleDisplayString => "🦛";

    public bool StopsBullet => true;

    public override bool BlocksMotion(Player p) => true;

    public Level Cycle(Level level, ActionInput actionInput)
    {
        if (!Health.HasHealth)
            return Behaviors.Die(level, this);
        else if (LockedToPlayer)
            return LockAbovePlayer(level);
        else
            return Behaviors.NoReaction(level);
    }

    private Level LockAbovePlayer(Level level)
    {
        Player player = level.Player;
        Location newLocation = new Location(Col: player.Location.Col, Row: player.Location.Row.NextNorth());
        return level.Replace(this, this with { Location = newLocation, LockedToPlayer = true });
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

