namespace HercAndHippoLibCs
{
    public interface IDisplayable
    {
        public Location Location { get;  }
        public ConsoleColor Color { get; }
        public ConsoleColor BackgroundColor { get; }
        public string ConsoleDisplayString { get; }
    }
    public interface IShootable { public Level OnShot(Level level, Direction shotFrom, Bullet shotBy); }
    public interface ITouchable { public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy); }
    public interface ICyclable { Level Cycle(Level level, ActionInput actionInput); }
    public interface ITakeable { Level OnTake(Level level); ConsoleColor Color { get; } }
}
