using System.Linq;

namespace HercAndHippoLibCs
{
    public record Level(IEnumerable<IDisplayable> Displayables)
    {
        public int MaxRow => Math.Min(Console.BufferHeight - 1, Displayables.Select(d => d.Location.Row).Max());
        public int MaxCol => Math.Max(Console.BufferWidth - 1, Displayables.Select(d => d.Location.Col).Max());
        public Location Corner => (MaxRow, MaxCol);

        // The following seem evil... don't love the design choice of relying on a Level object being tightly coupled to a Player object
        public Player FindPlayer() => (Player) Displayables.Where(d => d is Player p).Single();
        public Level WithPlayer(Player newPlayer) => this with { Displayables = Displayables.Where(d => d is not Player p).Append(newPlayer) };
    }

    public static class TestLevels
    {
        /*  WWWWWWWWW
            W       W
            W   P   W
            WWWWWWWWW */
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
            new Wall(Color.Green, (9,2)),

            new Wall(Color.Yellow, (1,3)),
            new Player((4,3), 100),
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

        public static readonly Level WallsLevel = new(wallsObjects);
    }


}
