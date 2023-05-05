using System;

namespace HercAndHippoLibCs
{
    public record Level(Location PlayerStart, IEnumerable<IDisplayable> Displayables);

    public static class TestLevels
    {
        /*  WWWWWWWWW
            W       W
            W   P   W
            WWWWWWWWW */
        private static List<IDisplayable> wallsObjects = new()
        {
            new Wall(Color.Yellow, (1,1)),
            new Wall(Color.Yellow, (2,1)),
            new Wall(Color.Yellow, (3,1)),
            new Wall(Color.Yellow, (4,1)),
            new Wall(Color.Yellow, (5,1)),
            new Wall(Color.Yellow, (6,1)),
            new Wall(Color.Yellow, (7,1)),
            new Wall(Color.Yellow, (8,1)),
            new Wall(Color.Yellow, (9,1)),

            new Wall(Color.Yellow, (1,2)),
            new Wall(Color.Yellow, (9,2)),

            new Wall(Color.Yellow, (1,3)),
            new Wall(Color.Yellow, (9,3)),

            new Wall(Color.Yellow, (1,4)),
            new Wall(Color.Yellow, (2,4)),
            new Wall(Color.Yellow, (3,4)),
            new Wall(Color.Yellow, (4,4)),
            new Wall(Color.Yellow, (5,4)),
            new Wall(Color.Yellow, (6,4)),
            new Wall(Color.Yellow, (7,4)),
            new Wall(Color.Yellow, (8,4)),
            new Wall(Color.Yellow, (9,4))
        };

        public static readonly Level WallsLevel = new(PlayerStart: (1, 4), wallsObjects);
    }

}
