using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCs
{
    public static class DemoLevels
    {
        public static readonly Level WallsLevel = new(
            player: new Player((4, 3), Health: 100, AmmoCount: 0, Inventory: Inventory.EmptyInventory),
            displayables: new HashSet<IDisplayable>
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
                new BreakableWall(ConsoleColor.Green, (9,2)),
                new Door(ConsoleColor.Cyan, (10,2)),

                new Wall(ConsoleColor.Yellow, (1,3)),
                new BreakableWall(ConsoleColor.Green, (9,3)),
                new Door(ConsoleColor.Magenta, (10,3)),

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

                new Ammo((3,10), Count: 20),
                new Ammo((4,10), Count: 20),
                new Ammo((5,10), Count: 20),

                // Downward-pointing arrow
                new BreakableWall(ConsoleColor.Magenta, (4, 12)),
                new BreakableWall(ConsoleColor.Magenta, (4, 13)),
                new BreakableWall(ConsoleColor.Magenta, (4, 14)),
                new BreakableWall(ConsoleColor.Magenta, (4, 15)),
                new BreakableWall(ConsoleColor.Magenta, (4, 16)),
                new BreakableWall(ConsoleColor.Magenta, (1, 17)),
                new BreakableWall(ConsoleColor.Magenta, (4, 17)),
                new BreakableWall(ConsoleColor.Magenta, (7, 17)),
                new BreakableWall(ConsoleColor.Magenta, (2, 18)),
                new BreakableWall(ConsoleColor.Magenta, (4, 18)),
                new BreakableWall(ConsoleColor.Magenta, (6, 18)),
                new BreakableWall(ConsoleColor.Magenta, (3, 19)),
                new BreakableWall(ConsoleColor.Magenta, (4, 19)),
                new BreakableWall(ConsoleColor.Magenta, (5, 19)),
                new BreakableWall(ConsoleColor.Magenta, (4, 20)),


                // Walls on the bottom to define level boundary
                new Wall(ConsoleColor.Blue, (1,40)),
                new Wall(ConsoleColor.Blue, (37, 40)),
                new Wall(ConsoleColor.Blue, (38, 40)),
                new Wall(ConsoleColor.Blue, (39, 40)),
                new BreakableWall(ConsoleColor.Blue, (40, 40))
            });

        public static readonly Level Clones = new(
            player: new Player((2, 2), Health: 100, AmmoCount: 200, Inventory: Inventory.EmptyInventory),
            displayables: new HashSet<IDisplayable>()
            {
                new Wall(ConsoleColor.DarkGreen, (100, 100)),
                new Player((5, 5), Health: 100, AmmoCount: 200, Inventory: Inventory.EmptyInventory),
                new Player((10, 10), Health: 100, AmmoCount: 200, Inventory: Inventory.EmptyInventory),
                new Player((20, 10), Health: 100, AmmoCount: 200, Inventory: Inventory.EmptyInventory),
                new Player((30, 10), Health: 100, AmmoCount: 200, Inventory: Inventory.EmptyInventory),
            });

        public static  Level SoManyBullets()
        {
            int width = 120;
            int height = 80;
            Player player = new((10, height - 1), Health: 100, AmmoCount: 200, Inventory: Inventory.EmptyInventory);
            HashSet<IDisplayable> displayables = new();
            for (Column col = 0; col <= width; col++)
            {
                displayables.Add(new Bullet((col, 2), Direction.South));
                displayables.Add(new Bullet((col, 12), Direction.South));
                displayables.Add(new Bullet((col, 22), Direction.South));
                displayables.Add(new Wall(ConsoleColor.Green, (col, height)));
            }
            return new Level(player, displayables);
        }

    }
}

