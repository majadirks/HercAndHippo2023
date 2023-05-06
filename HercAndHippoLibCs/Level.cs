namespace HercAndHippoLibCs
{
    public record Level(Player Player, IEnumerable<IDisplayable> Displayables)
    {
        public IEnumerable<IDisplayable> LevelObjects => Displayables.Append(Player);
        public Level WithPlayer(Player player) => this with { Player = player };
        public IEnumerable<IDisplayable> ObjectsAt(Location location) => LevelObjects.Where(d => d.Location.Equals(location));
        public Level Without(IDisplayable toRemove) => this with { Displayables = Displayables.Where(d => !d.Equals(toRemove)) };
        public Level AddObject(IDisplayable toAdd) => this with { Displayables = Displayables.Append(toAdd) };
        public Level RefreshCyclables(ConsoleKeyInfo keyInfo)
            => LevelObjects.Where(disp => disp is ICyclable cylable)
            .Select(c => (ICyclable)c)
            .Aggregate(seed: this, func: (state, nextCyclable) => nextCyclable.Cycle(state, keyInfo));
        public bool HasSameStateAs(Level otherState)
            => LevelObjects.Count() == otherState.LevelObjects.Count() &&
               LevelObjects.Zip(otherState.LevelObjects).All(zipped => zipped.First.Equals(zipped.Second));
    }

    public static class TestLevels
    {
        public static readonly Level WallsLevel = new(
            Player: new Player((4, 3), Health: 100, AmmoCount: 0, Inventory: new HashSet<ITakeable>()),
            Displayables: new IDisplayable[]
            {
                new Wall(ConsoleColor.Yellow, (1,1)),
                new Wall(ConsoleColor.Yellow, (2,1)),
                new Wall(ConsoleColor.Yellow, (3,1)),
                new Wall(ConsoleColor.Yellow, (4,1)),
                new Wall(ConsoleColor.Yellow, (5,1)),
                new Wall(ConsoleColor.Yellow, (6,1)),
                new Wall(ConsoleColor.Yellow, (7,1)),
                new Wall(ConsoleColor.Yellow, (8,1)),
                new Wall(ConsoleColor.Green, (9,1)),

                new Wall(ConsoleColor.Yellow, (1,2)),
                new Ammo((2,2), Count: 5),
                new Key(ConsoleColor.Magenta, (7,2)),
                new Wall(ConsoleColor.Green, (9,2)),

                new Wall(ConsoleColor.Yellow, (1,3)),
                new BreakableWall(ConsoleColor.Green, (9,3)),
                new Door(ConsoleColor.Magenta, (10,3)),
                new Door(ConsoleColor.Cyan, (11,3)),

                new Wall(ConsoleColor.Yellow, (1,4)),
                new Wall(ConsoleColor.Yellow, (2,4)),
                new Wall(ConsoleColor.Yellow, (3,4)),
                new Wall(ConsoleColor.Yellow, (4,4)),
                new Wall(ConsoleColor.Yellow, (5,4)),
                new Wall(ConsoleColor.Yellow, (6,4)),
                new Wall(ConsoleColor.Yellow, (7,4)),
                new Wall(ConsoleColor.Yellow, (8,4)),
                new Wall(ConsoleColor.Green, (9,4)),

                new Key(ConsoleColor.Cyan, (4,8)),
                
                new Ammo((3,10), Count: 5),
                new Ammo((4,10), Count: 5),
                new Ammo((5,10), Count: 5)
            });
    }


}
