namespace HercAndHippoLibCs
{
    public interface IDisplayable
    {
        public Location Location { get; }
        public ConsoleColor Color { get; }
        public string ConsoleDisplayString { get; }
    }
    public interface IShootable { public Level OnShot(Level level, Direction shotFrom, Bullet shotBy); }
    public interface ITouchable { public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy); }
    public interface ICyclable { Level Cycle(Level level, ConsoleKeyInfo keyInfo); }
    public interface ITakeable { Level OnTake(Level level); }
}
