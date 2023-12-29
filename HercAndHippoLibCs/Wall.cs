namespace HercAndHippoLibCs
{
    public record Wall(Color Color, Location Location) : HercAndHippoObj, ILocatable, ITouchable, IShootable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "█";
        public Color BackgroundColor => Color;

        public bool StopsBullet => true;

        public override bool BlocksMotion(Player p) => true;

        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.NoReaction(level);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => Behaviors.NoReaction(level);
    }
}
