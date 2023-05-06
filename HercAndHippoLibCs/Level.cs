namespace HercAndHippoLibCs
{
    public record Level(Player Player, IEnumerable<IDisplayable> Displayables)
    {
        public IEnumerable<IDisplayable> LevelObjects => Displayables.Append(Player);
        public int MaxRow => Math.Min(Console.BufferHeight - 1, Displayables.Select(d => d.Location.Row).Max());
        public int MaxCol => Math.Max(Console.BufferWidth - 1, Displayables.Select(d => d.Location.Col).Max());
        public Location Corner => (MaxRow, MaxCol);
        public Level WithPlayer(Player player) => this with { Player = player };
        public IDisplayable? ObjectAt(Location location) => Displayables.AsParallel().Where(d => d.Location.Equals(location)).FirstOrDefault();
        public Level Without(IDisplayable toRemove) => this with { Displayables = Displayables.Where(d => !d.Equals(toRemove)) };
        public Level AddObject(IDisplayable toAdd) => this with { Displayables = Displayables.Append(toAdd) };
        public Level RefreshCyclables(ConsoleKeyInfo keyInfo)
            => LevelObjects.Where(disp => disp is ICyclable cylable)
            .Select(c => (ICyclable)c)
            .Aggregate(seed: this, func: (oldState, nextCyclable) => nextCyclable.Cycle(oldState, keyInfo));
        public bool HasSameStateAs(Level otherState)
            => LevelObjects.Count() == otherState.LevelObjects.Count() &&
               LevelObjects.Zip(otherState.LevelObjects).All(zipped => zipped.First.Equals(zipped.Second));
    }

    public static class TestLevels
    {
        private static readonly List<IDisplayable> wallsObjects = new()
        {
            new Wall(Color.Yellow, (1,1)),
            new Wall(Color.Yellow, (2,1)),
            new Wall(Color.Yellow, (3,1)),
            new Wall(Color.Yellow, (4,1)),
            new Wall(Color.Yellow, (5,1)),
            new Wall(Color.Yellow, (6,1)),
            new Wall(Color.Yellow, (7,1)),
            new Wall(Color.Yellow, (8,1)),
            new Wall(Color.Green, (9,1)),

            new Wall(Color.Yellow, (1,2)),
            new Ammo((2,2), Count: 5),
            new Wall(Color.Green, (9,2)),

            new Wall(Color.Yellow, (1,3)),
            new BreakableWall(Color.Green, (9,3)),
            new Door(Color.Purple, (10,3)),

            new Wall(Color.Yellow, (1,4)),
            new Wall(Color.Yellow, (2,4)),
            new Wall(Color.Yellow, (3,4)),
            new Wall(Color.Yellow, (4,4)),
            new Wall(Color.Yellow, (5,4)),
            new Wall(Color.Yellow, (6,4)),
            new Wall(Color.Yellow, (7,4)),
            new Wall(Color.Yellow, (8,4)),
            new Wall(Color.Green, (9,4))
        };

        public static readonly Level WallsLevel = new(
            Player: new Player((4,3), Health: 100, AmmoCount: 0),
            Displayables: wallsObjects);
    }


}
