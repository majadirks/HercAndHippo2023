namespace HercAndHippoLibCsTest
{
    /// <summary>Increments a counter at each cycle </summary>
    internal record CycleCounter(Location Location, int Count) : HercAndHippoObj, ILocatable, ICyclable
    {
        public string ConsoleDisplayString => Count.ToString();
        private Level IncreaseCount(Level level) => level.Replace(this, this with { Count = Count + 1 });
        public Level Cycle(Level level, ActionInput actionInput) => IncreaseCount(level);
    }

    /// <summary>Increments a counter when touched </summary>
    internal record TouchCounter(Location Location,int Count) : HercAndHippoObj, ILocatable, ITouchable
    {
        private Level IncreaseCount(Level level) => level.Replace(this, this with { Count = Count + 1 });
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => IncreaseCount(level);
    }

    /// <summary>Increments a counter when shot </summary>
    internal record ShotCounter(Location Location, int Count) : HercAndHippoObj, ILocatable, IShootable
    {
        private Level IncreaseCount(Level level) => level.Replace(this, this with { Count = Count + 1 });
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => IncreaseCount(level);
    }

    /// <summary>Object that does not respond in any particular way to being shot or touched.</summary>
    internal record Noninteractor(Location Location) : HercAndHippoObj, ILocatable {}
}
