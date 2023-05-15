namespace HercAndHippoLibCsTest
{
    /// <summary>Increments a counter at each cycle </summary>
    internal record CycleCounter(Location Location, ConsoleColor Color, int Count) : IDisplayable, ICyclable
    {
        public string ConsoleDisplayString => Count.ToString();
        public ConsoleColor BackgroundColor => ConsoleColor.Black;
        private Level IncreaseCount(Level level) => level.Replace(this, this with { Count = Count + 1 });
        public Level Cycle(Level level, ActionInput actionInput) => IncreaseCount(level);
    }

    /// <summary>Increments a counter when touched </summary>
    internal record TouchCounter(Location Location, ConsoleColor Color, int Count) : IDisplayable, ITouchable
    {
        public string ConsoleDisplayString => Count.ToString();
        public ConsoleColor BackgroundColor => ConsoleColor.Black;
        private Level IncreaseCount(Level level) => level.Replace(this, this with { Count = Count + 1 });
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => IncreaseCount(level);
    }

    /// <summary>Increments a counter when shot </summary>
    internal record ShotCounter(Location Location, ConsoleColor Color, int Count) : IDisplayable, IShootable
    {
        public string ConsoleDisplayString => Count.ToString();
        public ConsoleColor BackgroundColor => ConsoleColor.Black;
        private Level IncreaseCount(Level level) => level.Replace(this, this with { Count = Count + 1 });
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => IncreaseCount(level);
    }
}
