namespace HercAndHippoLibCs;

public record Hippo(Location Location, Health Health) : HercAndHippoObj, ILocatable, ITouchable, ICyclable, IShootable, IConsoleDisplayable
{
    public Color Color => Color.Magenta;

    public Color BackgroundColor => Color.Black;

    public string ConsoleDisplayString => "🦛";

    public bool StopsBullet => true;

    public override bool BlocksMotion(Player p) => true;

    public Level Cycle(Level level, ActionInput actionInput)
    {
        // ToDo
        return Behaviors.NoReaction(level);
    }

    public Level OnShot(Level level, Direction shotFrom, Bullet shotBy)
        => level.Without(shotBy).Replace(this, this with { Health = this.Health - 5 });

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        // ToDo
        return Behaviors.NoReaction(level);
    }
}

