﻿namespace HercAndHippoLibCs;

public static class DemoLevels
{
    public static readonly Level WallsLevel = new(
        player: new Player((4, 3), health: 100, ammoCount: 0, inventory: Inventory.EmptyInventory, jumpStrength: 5),
        gravity: 0,
        secondaryObjects: new HashSet<HercAndHippoObj>
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
            new Key(Color.Magenta, (7,2)),
            new BreakableWall(Color.Green, (9,2)),
            new Door(Color.Cyan, (10,2)),

            new Wall(Color.Yellow, (1,3)),
            new BreakableWall(Color.Green, (9,3)),
            new Door(Color.Magenta, (10,3)),

            new Wall(Color.Yellow, (1,4)),
            new Wall(Color.Yellow, (2,4)),
            new Wall(Color.Yellow, (3,4)),
            new Wall(Color.Yellow, (4,4)),
            new Wall(Color.Yellow, (5,4)),
            new Wall(Color.Yellow, (6,4)),
            new Wall(Color.Yellow, (7,4)),
            new Wall(Color.Yellow, (8,4)),
            new Wall(Color.Green, (9,4)),

            new Key(Color.Cyan, (4,8)),

            new Ammo((3,10), Count: 20),
            new Ammo((4,10), Count: 20),
            new Ammo((5,10), Count: 20),

            // Downward-pointing arrow
            new BreakableWall(Color.Magenta, (4, 12)),
            new BreakableWall(Color.Magenta, (4, 13)),
            new BreakableWall(Color.Magenta, (4, 14)),
            new BreakableWall(Color.Magenta, (4, 15)),
            new BreakableWall(Color.Magenta, (4, 16)),
            new BreakableWall(Color.Magenta, (1, 17)),
            new BreakableWall(Color.Magenta, (4, 17)),
            new BreakableWall(Color.Magenta, (7, 17)),
            new BreakableWall(Color.Magenta, (2, 18)),
            new BreakableWall(Color.Magenta, (4, 18)),
            new BreakableWall(Color.Magenta, (6, 18)),
            new BreakableWall(Color.Magenta, (3, 19)),
            new BreakableWall(Color.Magenta, (4, 19)),
            new BreakableWall(Color.Magenta, (5, 19)),
            new BreakableWall(Color.Magenta, (4, 20)),


            // Walls on the bottom to define level boundary
            new Wall(Color.Blue, (1,40)),
            new Wall(Color.Blue, (37, 40)),
            new Wall(Color.Blue, (38, 40)),
            new Wall(Color.Blue, (39, 40)),
            new BreakableWall(Color.Blue, (40, 40)),
            new BreakableWall(Color.Blue, (50, 40)),
            new BreakableWall(Color.Blue, (60, 40)),
            new BreakableWall(Color.Blue, (70, 40)),
            new BreakableWall(Color.Blue, (80, 40)),
            new BreakableWall(Color.Blue, (90, 40)),
            new BreakableWall(Color.Blue, (100, 40)),
            new BreakableWall(Color.Blue, (110, 40)),
            new BreakableWall(Color.Blue, (120, 40)),
            new BreakableWall(Color.Blue, (130, 40)),
            new BreakableWall(Color.Blue, (140, 40)),
            new BreakableWall(Color.Blue, (150, 40)),
            new BreakableWall(Color.Blue, (160, 40)),
            new BreakableWall(Color.Blue, (170, 40)),
            new BreakableWall(Color.Blue, (180, 40)),
            new BreakableWall(Color.Blue, (190, 40)),
            new BreakableWall(Color.Blue, (200, 40)),
            new BreakableWall(Color.Blue, (210, 40)),
            new BreakableWall(Color.Blue, (220, 40)),
            new BreakableWall(Color.Blue, (230, 40)),
            new BreakableWall(Color.Blue, (240, 40)),
            new BreakableWall(Color.Blue, (250, 40)),
            new BreakableWall(Color.Blue, (260, 40)),
            new BreakableWall(Color.Blue, (270, 40)),
            new BreakableWall(Color.Blue, (280, 40)),
            new BreakableWall(Color.Blue, (290, 40)),
            new BreakableWall(Color.Blue, (300, 40))
        });

    public static readonly Level Clones = new(
        player: new Player((2, 2), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5, kineticEnergy: 0),
        gravity: 0,
        secondaryObjects: new HashSet<HercAndHippoObj>()
        {
            new Wall(Color.DarkGreen, (100, 100)),
            new Player((5, 5), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5),
            new Player((10, 10), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5),
            new Player((20, 10), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5),
            new Player((30, 10), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5),
        });

    public static  Level SoManyBullets()
    {
        int width = 120;
        int height = 120;
        Player player = new((10, height - 1), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5);
        HashSet<HercAndHippoObj> displayables = new();
        for (Column col = 0; col <= width; col++)
        {
            displayables.Add(new Bullet((col, 2), Direction.South));
            displayables.Add(new Bullet((col, 12), Direction.South));
            displayables.Add(new Bullet((col, 22), Direction.South));
            displayables.Add(new Wall(Color.Green, (col, height)));
        }
        for (int row = 0; row <= height; row++)
        {
            displayables.Add(new BreakableWall(Color.Blue, (row % 5, row)));
        }
        displayables.Add(new Driver(Direction.North, Modulus: 3));
        displayables.Add(new Bullet((15, height - 1), Direction.Idle));
        return new Level(player, secondaryObjects: displayables, gravity: 0);
    }

    public static readonly Level JumpLevel = new(
        player: new Player(location: new(5, 10), health: 100, ammoCount: 100, inventory: Inventory.EmptyInventory, jumpStrength: 5),
        secondaryObjects: new HashSet<HercAndHippoObj>()
        {
            new Wall(Color.Blue, (1,11)),
            new Wall(Color.Blue, (4,11)),
            new Wall(Color.Blue, (5,11)),
            new Wall(Color.Blue, (6,11)),
            new Wall(Color.Blue, (7,11)),
            new Wall(Color.Blue, (8,11)),
            new Wall(Color.Blue, (9,11)),
            new Wall(Color.Blue, (10,11)),

            new Wall(Color.Blue, ( 5,15)),
            new Wall(Color.Blue, ( 6,15)),
            new Wall(Color.Blue, ( 7,15)),
            new Wall(Color.Blue, ( 8,15)),
            new Wall(Color.Blue, ( 9,15)),
            new Wall(Color.Blue, (10,15)),
            new Wall(Color.Blue, (11,15)),
            new Wall(Color.Blue, (12,15)),
            new Wall(Color.Blue, (13,15)),
            new Wall(Color.Blue, (14,15)),
            new Wall(Color.Blue, (15,15)),

            new Wall(Color.Blue, ( 1,20)),
            new Wall(Color.Blue, ( 2,20)),
            new Wall(Color.Blue, ( 3,20)),
            new Wall(Color.Blue, ( 4,20)),
            new Wall(Color.Blue, ( 5,20)),
            new Wall(Color.Blue, ( 6,20)),
            new Wall(Color.Blue, ( 7,20)),
            new Wall(Color.Blue, ( 8,20)),
            new Wall(Color.Blue, ( 9,20)),
            new Wall(Color.Blue, (10,20)),
        },
        gravity: 1);

}

