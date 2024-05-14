

using System.ComponentModel;
using System.Diagnostics.Metrics;

namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Bullet_Tests
    {

        [TestMethod]
        public void BulletCallsOnShot_Test()
        {
            // Arrange
            int initialCount = 0;
            Player player = new((1, 1), health: 100, ammoCount: 0, inventory: Inventory.EmptyInventory);
            ShotCounter initialCounter = new((3,3), initialCount);
            ShotCounter cycledCounter = initialCounter with { Count = initialCount + 1 };
            Bullet bullet = new((2, 3), Direction.East);
            Bullet movedBullet = bullet with { Location = initialCounter.Location };
            Level level = new(player: player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { bullet, initialCounter });
            Assert.IsTrue(level.Contains(bullet));
            Assert.IsFalse(level.Contains(movedBullet));
            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));

            // Act
            // Bullet is initially left of the counter. It moves to cover the counter.
            // OnShot() is not yet called.
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert
            Assert.IsFalse(level.Contains(bullet));
            Assert.IsTrue(level.Contains(movedBullet));
            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));

            // Act
            // OnShot() for counter is called, incrementing its count
            level = level.RefreshCyclables(ActionInput.NoAction);
            Assert.IsFalse(level.Contains(initialCounter));
            Assert.IsTrue(level.Contains(cycledCounter));
        }

        [TestMethod]
        public void PlayerCanShootAdjacentObjects_Test()
        {
            // Arrange
            int initialCount = 0;
            Player player = new((3, 2), health: 100, ammoCount: 1, inventory: Inventory.EmptyInventory);
            ShotCounter initialCounter = new ShotCounter((2, 2), Count: initialCount).ForgetId();
            ShotCounter cycledCounter = initialCounter with { Count = initialCount + 1 };
            Level level = new(player, 
                gravity: Gravity.None, 
                secondaryObjects: new HashSet<HercAndHippoObj>() 
                { 
                    initialCounter, 
                    new Wall(Color.Black, (20, 20)) 
                });
            level = level.ForgetIds();

            Bullet bullet = new Bullet(initialCounter.Location, Direction.West).ForgetId();
            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));
            Assert.IsFalse(level.Contains(bullet));
            // Act: Player shoots at the counter immediately west of it.
            // Cycle twice. On first cycle, player places bullet over the counter. On second cycle, the bullet
            // calls the counter's OnShot method and then moves.
            // If this behavior changes in the future (ie if I decide that the bullet should call OnShot
            // when it is first created), this test will still pass, which is fine; I just want to enforce
            // that OnShot is called at some point before the bullet moves past the object.
            level = level.RefreshCyclables(ActionInput.ShootWest).ForgetIds();
            Assert.IsTrue(level.Contains(bullet));
            level = level.RefreshCyclables(ActionInput.NoAction).ForgetIds();
            // Assert: counter has cycled
            Assert.IsFalse(level.Contains(initialCounter));
            Assert.IsTrue(level.Contains(cycledCounter));
        }

        [TestMethod]
        public void BulletMovesPastNonShootableObjects_Test()
        {
            // Arrange
            Noninteractor nonshootable = new((3, 3));
            Bullet initialBullet = new((2, 3), Direction.East);
            Bullet bullet2 = new(nonshootable.Location, Direction.East);
            Bullet bullet3 = new((4, 3), Direction.East);
            Wall corner = new(Color.Yellow, (10, 10)); // Give bullet room in the level to move past the nonshootable
            Player player = new((3, 2), health: 100, ammoCount: 1, inventory: Inventory.EmptyInventory);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { initialBullet, nonshootable, corner });

            // Act and assert
            level = level.RefreshCyclables(ActionInput.NoAction);
            Assert.IsFalse(level.Contains(initialBullet));
            Assert.IsTrue(level.Contains(bullet2));
            Assert.IsTrue(level.Contains(nonshootable));

            level = level.RefreshCyclables(ActionInput.NoAction);
            Assert.IsFalse(level.Contains(bullet2));
            Assert.IsTrue(level.Contains(bullet3));
            Assert.IsTrue(level.Contains(nonshootable));

        }

        [TestMethod]
        public void BulletMovesInCorrectDirection_Test()
        {
            // Arrange
            Wall corner = new(Color.Yellow, (10, 10));
            Bullet initialEastward = new((5,5), Direction.East);
            Bullet cycledEastward = new((6, 5), Direction.East);
            Bullet initialNorthward = new((5, 5), Direction.North);
            Bullet cycledNorthward = new((5, 4), Direction.North);
            Bullet initialSouthward = new((5, 5), Direction.South);
            Bullet cycledSouthward = new((5, 6), Direction.South);
            Bullet initialWestward = new((5, 5), Direction.West);
            Bullet cycledWestward = new((4, 5), Direction.West);
            Player player = new((1, 1), health: 100, ammoCount: 5, inventory: Inventory.EmptyInventory);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { corner, initialEastward, initialWestward, initialNorthward, initialSouthward });

            // Act
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert
            Assert.IsTrue(level.Contains(cycledEastward));
            Assert.IsTrue(level.Contains(cycledNorthward));
            Assert.IsTrue(level.Contains(cycledWestward));
            Assert.IsTrue(level.Contains(cycledSouthward));     
        }

        [TestMethod]
        public void BulletDiesAtEastEdgeOfLevel_Test()
        {
            // Arrange
            int cornerCol = 10;
            int bulletRow = 3;
            Wall corner = new(Color.Yellow, (cornerCol, 10));
            Bullet bullet = new((7, bulletRow), Direction.East);
            Bullet bulletAtEdge = bullet with { Location = (cornerCol, bulletRow) };
            Bullet bulletBeyondEdge = bullet with { Location = (cornerCol + 1, bulletRow) };
            Level level = new(Player.Default(1, 1), gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() {bullet, corner});
            // Act
            while (!level.Contains(bulletAtEdge)) // Bullet moves until it reaches east edge
                level = level.RefreshCyclables(ActionInput.NoAction);                
            Assert.IsTrue(level.Contains(bulletAtEdge));

            // Let it move once more
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert: no more bullet
            Assert.IsFalse(level.Contains(bulletAtEdge));
            Assert.IsFalse(level.Contains(bulletBeyondEdge));
        }

        [TestMethod]
        public void BulletDiesAtWestEdgeOfLevel_Test()
        {
            // Arrange
            int cornerCol = 10;
            int bulletRow = 3;
            Wall corner = new(Color.Yellow, (cornerCol, 10));
            Bullet bullet = new((5, bulletRow), Direction.West);
            Bullet bulletAtEdge = bullet with { Location = (Column.MIN_COL, bulletRow) };
            Level level = new(Player.Default(5, 1), gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { bullet, corner });
            // Act
            while (!level.Contains(bulletAtEdge)) // Bullet moves until it reaches east edge
                level = level.RefreshCyclables(ActionInput.NoAction);
            Assert.AreEqual(bulletAtEdge, level.LevelObjects.Where(b => b is Bullet).Single());

            // Let it move once more
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert: no more bullet
            Assert.IsFalse(level.LevelObjects.Where(b => b is Bullet).Any());
        }

        [TestMethod]
        public void BulletDiesAtSouthEdgeOfLevel_Test()
        {
            // Arrange
            int cornerRow = 10;
            Wall corner = new(Color.Yellow, (10, cornerRow));
            Bullet bullet = new((7, 2), Direction.South);
            Bullet bulletAtEdge = bullet with { Location = (7, 10) };
            Bullet bulletBeyondEdge = bullet with { Location = (7, 11) };
            Level level = new(Player.Default(1, 1), gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { bullet, corner });
            // Act
            while (!level.Contains(bulletAtEdge)) // Bullet moves until it reaches south edge     
                level = level.RefreshCyclables(ActionInput.NoAction); 
            Assert.IsTrue(level.Contains(bulletAtEdge));
            // Let it move once more
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert: no more bullet
            Assert.IsFalse(level.Contains(bulletAtEdge));
            Assert.IsFalse(level.Contains(bulletBeyondEdge));
        }

        [TestMethod]
        public void BulletDiesAtNorthEdgeOfLevel_Test()
        {
            // Arrange
            int cornerRow = 10;
            Wall corner = new(Color.Yellow, (10, cornerRow));
            Bullet bullet = new((7, 2), Direction.North);
            Bullet bulletAtEdge = bullet with { Location = (7, Row.MIN_ROW) };
            Bullet bulletBeyondEdge = bullet with { Location = (7, 11) };
            Level level = new(Player.Default(7, 3), gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { bullet, corner });
            // Act
            while (!level.Contains(bulletAtEdge)) // Bullet moves until it reaches south edge     
                level = level.RefreshCyclables(ActionInput.NoAction);
            Assert.AreEqual(bulletAtEdge, level.LevelObjects.Where(b => b is Bullet).Single());
           
            // Let it move once more
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert: no more bullet
            Assert.IsFalse(level.LevelObjects.Where(b => b is Bullet).Any());
        }


        [TestMethod]
        public void BulletDiesWhenPlacedBeyondLevelEdge_Test()
        {
            // Arrange
            Wall corner1 = new(Color.Yellow, (10, 9));
            Wall corner2 = new(Color.Yellow, (9, 10));
            Player player = Player.Default(10,10); //placed in bottom-right corner
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { corner1, corner2 });

            // Act and assert
            // Shoot east and then cycle; there should be no bullet remaining.
            level = level.RefreshCyclables(ActionInput.ShootEast).RefreshCyclables(ActionInput.NoAction);
            Assert.IsFalse(level.LevelObjects.Where(obj => obj is Bullet).Any());
            // Shoot south and then cycle; there should be no bullet remaining.
            level = level.RefreshCyclables(ActionInput.ShootSouth).RefreshCyclables(ActionInput.NoAction);
            Assert.IsFalse(level.LevelObjects.Where(obj => obj is Bullet).Any());
        }

        [TestMethod]
        public void CanShootNorthAndSouthInFirstColumn()
        {
            // Arrange
            Player player = new((Column.MIN_COL, 4), health: 10, ammoCount: 2, inventory: Inventory.EmptyInventory);
            ShotCounter northCounter = new((Column.MIN_COL, 1), Count: 0);
            ShotCounter southCounter = new((Column.MIN_COL, 7), Count: 0);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { northCounter, southCounter });
            // Act
            level = level
                .RefreshCyclables(ActionInput.ShootNorth)
                .RefreshCyclables(ActionInput.ShootSouth)
                .RefreshCyclables(ActionInput.NoAction)
                .RefreshCyclables(ActionInput.NoAction)
                .RefreshCyclables(ActionInput.NoAction);
            // Assert
            Assert.IsTrue(level.Contains(northCounter with { Count = 1 }));
            Assert.IsTrue(level.Contains(southCounter with { Count = 1 }));
            Assert.IsFalse(level.LevelObjects.Where(b => b is Bullet).Any());
        }

        [TestMethod]
        public void CanShootEastAndWestInFirstRow()
        {
            // Arrange
            Player player = new((4, Row.MIN_ROW), health: 10, ammoCount: 2, inventory: Inventory.EmptyInventory);
            ShotCounter westCounter = new((1, Row.MIN_ROW), Count: 0);
            ShotCounter eastCounter = new((7, Row.MIN_ROW), Count: 0);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { westCounter, eastCounter });
            // Act
            level = level
                .RefreshCyclables(ActionInput.ShootWest)
                .RefreshCyclables(ActionInput.ShootEast)
                .RefreshCyclables(ActionInput.NoAction)
                .RefreshCyclables(ActionInput.NoAction)
                .RefreshCyclables(ActionInput.NoAction);
            // Assert
            Assert.IsTrue(level.Contains(westCounter with { Count = 1 }));
            Assert.IsTrue(level.Contains(eastCounter with { Count = 1 }));
            Assert.IsFalse(level.LevelObjects.Where(b => b is Bullet).Any());
        }

        [TestMethod]
        public void CanShootBottomRightCorner_Test()
        {
            // Arrange
            Player player = new((4, 2), health: 100, ammoCount: 5, inventory: Inventory.EmptyInventory);
            ShotCounter counter = new((4, 4), 0);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { counter });

            // Act and assert

            // Shoot south. Count increments to 1.
            level = level
                .RefreshCyclables(ActionInput.ShootSouth)
                .RefreshCyclables(ActionInput.NoAction)
                .RefreshCyclables(ActionInput.NoAction);
            Assert.IsTrue(level.Contains(counter with { Count = 1 }));

            // Move west and south, so positioned west of counter, and shoot east.
            // Count increments to 2.
            level = level
                .WithPlayer(level.Player with { Location = (2, 4) })
                .RefreshCyclables(ActionInput.ShootEast)
                .RefreshCyclables(ActionInput.NoAction)
                .RefreshCyclables(ActionInput.NoAction);
            Assert.IsTrue(level.Contains(counter with { Count = 2 }));
        }

        [TestMethod]
        public void CanShootFromBottomRightCorner_Test()
        {
            // Arrange
            Player player = new((10, 10), health: 100, ammoCount: 5, inventory: Inventory.EmptyInventory);
            Noninteractor corner = new((10,10)); // Just to be perverse
            ShotCounter topRight = new((10, Row.MIN_ROW), 0);
            ShotCounter bottomLeft = new((Column.MIN_COL, 10), 0);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> {corner, topRight, bottomLeft });
            Assert.AreEqual(10, level.Width);
            Assert.AreEqual(10, level.Height);

            // Act
            level = level.RefreshCyclables(ActionInput.ShootWest).RefreshCyclables(ActionInput.ShootNorth);
            int maxWait = 15;
            int i = 0;
            while (level.LevelObjects.Where(b => b is Bullet).Any())
            {
                level = level.RefreshCyclables(ActionInput.NoAction);
                i++;
                if (i > maxWait) Assert.IsTrue(false);
            }

            //Assert
            Assert.IsTrue(level.Contains(topRight with { Count = 1 }));
            Assert.IsTrue(level.Contains(bottomLeft with { Count = 1 }));
        }

        [TestMethod]
        public void BulletSeeksWest_Test()
        {
            // Arrange
            Level level = new(
                player: Player.Default(5, 1),
                hippo: null,
                gravity: Gravity.Default,
                secondaryObjects: new()
                {
                    new Wall(Color.Yellow, (5,2)),
                    new Wall(Color.Black, (20,20)),
                    new Bullet((10, 1), Direction.Seek)
                });
            Bullet initial = FindBullet();
            Assert.AreEqual(new Location(10, 1), initial.Location);
            Assert.AreEqual(Direction.Seek, initial.Whither);

            // Act
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert
            Bullet iterated = FindBullet();
            Assert.AreEqual(new Location(9, 1), iterated.Location);
            Assert.AreEqual(Direction.West, iterated.Whither);

            // Local method
            Bullet FindBullet() => (Bullet)level.LevelObjects.Where(obj => obj is Bullet).Single();
        }

    }
}
