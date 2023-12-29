namespace HercAndHippoLibCs
{
    public interface ILocatable
    {
        public Location Location { get;  }
    }

    public interface IConsoleDisplayable : ILocatable
    {
        public Color Color { get; }
        public Color BackgroundColor { get; }
        public string ConsoleDisplayString { get; }
    }
    public interface IShootable: ILocatable 
    { 
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy); 
        public bool StopsBullet { get; }
    }
    public interface ITouchable: ILocatable { public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy); }
    public interface ICyclable { Level Cycle(Level level, ActionInput actionInput); }
    public interface ITakeable: ILocatable 
    { 
        Level OnTake(Level level); 
        string Id { get; } 
    }
}
