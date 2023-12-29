namespace HercAndHippoLibCs
{
    public record BreakableWall(Color Color, Location Location) : HercAndHippoObj, ILocatable, IShootable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "▓";
        public Color BackgroundColor => Color.Black;

        public bool StopsBullet => true;

        public override bool BlocksMotion(Player p) => true;

        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.Die(level, this);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => Behaviors.NoReaction(level);    
    }
}
