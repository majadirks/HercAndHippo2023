namespace HercAndHippoLibCs;

public static class DemoLevels
{
    public static readonly Level WallsLevel = new(
        player: new Player((4, 3), health: 100, ammoCount: 0, inventory: Inventory.EmptyInventory, jumpStrength: 5),
        gravity: Gravity.None,
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
        gravity: Gravity.None,
        secondaryObjects: new HashSet<HercAndHippoObj>()
        {
            new Wall(Color.DarkGreen, (100, 100)),
            new Player((5, 5), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5),
            new Player((10, 10), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5),
            new Player((20, 10), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5),
            new Player((30, 10), health: 100, ammoCount: 200, inventory: Inventory.EmptyInventory, jumpStrength: 5),
        });

    public static  Level ManyObjectsStressTest()
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
        return new Level(player, secondaryObjects: displayables, gravity: Gravity.None);
    }

    public static readonly Level JumpLevel = new(
        player: new Player(location: new(5, 10), health: 100, ammoCount: 100, inventory: Inventory.EmptyInventory, jumpStrength: 5),
        gravity: Gravity.Default,
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

            new Wall(Color.Blue, (40,16)),

            new Wall(Color.Blue, ( 1,18)),
            new Wall(Color.Blue, ( 2,18)),
            new Wall(Color.Blue, ( 3,18)),
            new Wall(Color.Blue, ( 4,18)),
            new Wall(Color.Blue, ( 5,18)),
            new Wall(Color.Blue, ( 6,18)),
            new Wall(Color.Blue, ( 7,18)),
            new Wall(Color.Blue, ( 8,18)),
            new Wall(Color.Blue, ( 9,18)),
            new Wall(Color.Blue, (10,18)),
            new Wall(Color.Blue, (11,18)),
            new Wall(Color.Blue, (12,18)),
            new Wall(Color.Blue, (13,18)),
            new Wall(Color.Blue, (14,18)),
            new Wall(Color.Blue, (15,18)),
            new Wall(Color.Blue, (16,18)),
            new Wall(Color.Blue, (17,18)),
            new Wall(Color.Blue, (18,18)),
            new Wall(Color.Blue, (19,18)),
            new Wall(Color.Blue, (20,18)),
            new Wall(Color.Blue, (21,18)),
            new Wall(Color.Blue, (22,18)),
            new Wall(Color.Blue, (23,18)),
            new Wall(Color.Blue, (24,18)),
            new Wall(Color.Blue, (25,18)),
            new Wall(Color.Blue, (26,18)),
            new Wall(Color.Blue, (27,18)),
            new Wall(Color.Blue, (28,18)),
            new Wall(Color.Blue, (29,18)),
            new Wall(Color.Blue, (30,18)),
            new Wall(Color.Blue, (31,18)),
            new Wall(Color.Blue, (32,18)),
            new Wall(Color.Blue, (33,18)),
            new Wall(Color.Blue, (34,18)),
            new Wall(Color.Blue, (35,18)),
            new Wall(Color.Blue, (36,18)),
            new Wall(Color.Blue, (37,18)),
            new Wall(Color.Blue, (38,18)),
            new Wall(Color.Blue, (39,18)),
            new Wall(Color.Blue, (40,18)),
            new Wall(Color.Blue, (41,18)),
            new Wall(Color.Blue, (42,18)),
            new Wall(Color.Blue, (43,18)),
            new Wall(Color.Blue, (44,18)),
            new Wall(Color.Blue, (45,18)),
            new Wall(Color.Blue, (46,18)),
            new Wall(Color.Blue, (47,18)),
            new Wall(Color.Blue, (48,18)),
            new Wall(Color.Blue, (49,18)),
            new Wall(Color.Blue, (50,18)),

            new Ammo((12,11), Count: 5),
            new Ammo((12, 12), Count: 5),
            new Ammo((12,13), Count: 5),
            new Ammo((12,14), Count: 5)
        });

    public static readonly Level IntroducingTheHippo = new(
        player: Player.Default(5, 10) with { JumpStrength = 6 },
        hippo: new Hippo((3, 10), Health: 15, LockedToPlayer: false),
        gravity: Gravity.Default,
        secondaryObjects: new()
        {
            new Ammo((7, 10), Count: 500),

            new Gem(Color.Red, (9, 10), Health: 5),

            new Wall(Color.Cyan, (1, 11)),
            new Wall(Color.Cyan, (2, 11)),
            new Wall(Color.Cyan, (3, 11)),
            new Wall(Color.Cyan, (4, 11)),
            new Wall(Color.Cyan, (5, 11)),
            new Wall(Color.Cyan, (6, 11)),
            new Wall(Color.Cyan, (7, 11)),
            new Wall(Color.Cyan, (8, 11)),
            new Wall(Color.Cyan, (9, 11)),
            new Wall(Color.Cyan, (10, 11)),
            new Wall(Color.Cyan, (11, 11)),
            new Wall(Color.Cyan, (12, 11)),
            new Wall(Color.Cyan, (13, 11)),
            new Wall(Color.Cyan, (14, 11)),
            new Wall(Color.Cyan, (15, 11)),
            new Wall(Color.Cyan, (16, 11)),
            new Wall(Color.Cyan, (17, 11)),
            new Wall(Color.Cyan, (18, 11)),
            new Wall(Color.Cyan, (19, 11)),
            new Wall(Color.Cyan, (20, 11)),
            new Wall(Color.Cyan, (21, 11)),
            new Wall(Color.Cyan, (22, 11)),
            new Wall(Color.Cyan, (23, 11)),
            new Wall(Color.Cyan, (24, 11)),
            new Wall(Color.Cyan, (25, 11)),
            new Wall(Color.Cyan, (26, 11)),
            new Wall(Color.Cyan, (27, 11)),
            new Wall(Color.Cyan, (28, 11)),
            new Wall(Color.Cyan, (29, 11)),
            new Wall(Color.Cyan, (30, 11)),

            new Gem(Color.DarkMagenta, Location: (1,4), Health: 5),
            new BreakableWall(Color.Green, (1, 5)),
            new BreakableWall(Color.Green, (2, 6)),
            new BreakableWall(Color.Green, (3, 7)),
            new Gem(Color.Red, (5,7), Health: 5),

            new Wall(Color.White, (9, 8)),
            new Wall(Color.White, (18, 9)),
            new Wall(Color.White, (22, 10)),

            new BreakableWall(Color.Green,(27,10)),
            new BreakableWall(Color.Green,(28,9)),
            new BreakableWall(Color.Green,(29,8)),
            new BreakableWall(Color.Green,(30,8)),
            new BreakableWall(Color.Green,(31,8)),
            new BreakableWall(Color.Green,(32,8)),
            new BreakableWall(Color.Green,(33,8)),
            new BreakableWall(Color.Green,(34,8)),
            new BreakableWall(Color.Green,(35,8)),
            new BreakableWall(Color.Green,(36,8)),
            new BreakableWall(Color.Green,(37,8)),
            new BreakableWall(Color.Green,(38,8)),
            new BreakableWall(Color.Green,(39,8)),
            new BreakableWall(Color.Green,(40,8)),
            new BreakableWall(Color.Green,(41,8)),
            new BreakableWall(Color.Green,(42,8)),
            new BreakableWall(Color.Green,(43,8)),
            new BreakableWall(Color.Green,(44,8)),
            new BreakableWall(Color.Green,(45,8)),
            new BreakableWall(Color.Green,(46,8)),
            new BreakableWall(Color.Green,(47,8)),
            new BreakableWall(Color.Green,(48,8)),
            new BreakableWall(Color.Green,(49,8)),
            new BreakableWall(Color.Green,(50,8)),
            new BreakableWall(Color.Green,(52,8)),

            new BreakableWall(Color.Green,(30,15)),
            new BreakableWall(Color.Green,(31,15)),
            new BreakableWall(Color.Green,(32,15)),
            new BreakableWall(Color.Green,(33,15)),
            new BreakableWall(Color.Green,(34,15)),
            new BreakableWall(Color.Green,(35,15)),
            new BreakableWall(Color.Green,(36,15)),
            new BreakableWall(Color.Green,(37,15)),
            new BreakableWall(Color.Green,(38,15)),
            new BreakableWall(Color.Green,(39,15)),
            new BreakableWall(Color.Green,(40,15)),
            new BreakableWall(Color.Green,(41,15)),
            new BreakableWall(Color.Green,(42,15)),
            new BreakableWall(Color.Green,(43,15)),
            new BreakableWall(Color.Green,(44,15)),
            new BreakableWall(Color.Green,(45,15)),
            new BreakableWall(Color.Green,(46,15)),
            new BreakableWall(Color.Green,(47,15)),
            new BreakableWall(Color.Green,(48,15)),
            new BreakableWall(Color.Green,(49,15)),
            new BreakableWall(Color.Green,(50,15)),
            new BreakableWall(Color.Green,(51,15)),
            new BreakableWall(Color.Green,(52,15)),

            new Wall(Color.Yellow, (400, 1))
        }
        );
}

