﻿

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
            Player player = new((1, 1), Health: 100, AmmoCount: 0, Inventory: Inventory.EmptyInventory);
            ShotCounter initialCounter = new((3,3), ConsoleColor.Blue, initialCount);
            ShotCounter cycledCounter = initialCounter with { Count = initialCount + 1 };
            Bullet bullet = new((2, 3), Direction.East);
            Bullet movedBullet = bullet with { Location = initialCounter.Location };
            Level level = new(player: player, displayables: new HashSet<IDisplayable> { bullet, initialCounter });
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
            Player player = new((3, 2), Health: 100, AmmoCount: 1, Inventory: Inventory.EmptyInventory);
            ShotCounter initialCounter = new((2, 2), ConsoleColor.Green, Count: initialCount);
            ShotCounter cycledCounter = initialCounter with { Count = initialCount + 1 };
            Level level = new(player, new HashSet<IDisplayable>() { initialCounter });
            Bullet bullet = new(initialCounter.Location, Direction.West);
            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));
            Assert.IsFalse(level.Contains(bullet));
            // Act: Player shoots at the counter immediately west of it.
            // Cycle twice. On first cycle, player places bullet over the counter. On second cycle, the bullet
            // calls the counter's OnShot method and then moves.
            // If this behavior changes in the future (ie if I decide that the bullet should call OnShot
            // when it is first created), this test will still pass, which is fine; I just want to enforce
            // that OnShot is called at some point before the bullet moves past the object.
            level = level.RefreshCyclables(ActionInput.ShootWest);
            Assert.IsTrue(level.Contains(bullet));
            level = level.RefreshCyclables(ActionInput.NoAction);
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
            Wall corner = new(ConsoleColor.Yellow, (10, 10)); // Give bullet room in the level to move past the nonshootable
            Player player = new((3, 2), Health: 100, AmmoCount: 1, Inventory: Inventory.EmptyInventory);
            Level level = new(player, new HashSet<IDisplayable> { initialBullet, nonshootable, corner });

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
            Wall corner = new(ConsoleColor.Yellow, (10, 10));
            Bullet initialEastward = new((5,5), Direction.East);
            Bullet cycledEastward = new((6, 5), Direction.East);
            Bullet initialNorthward = new((5, 5), Direction.North);
            Bullet cycledNorthward = new((5, 4), Direction.North);
            Bullet initialSouthward = new((5, 5), Direction.South);
            Bullet cycledSouthward = new((5, 6), Direction.South);
            Bullet initialWestward = new((5, 5), Direction.West);
            Bullet cycledWestward = new((4, 5), Direction.West);
            Player player = new((1, 1), Health: 100, AmmoCount: 5, Inventory: Inventory.EmptyInventory);
            Level level = new(player, new HashSet<IDisplayable>() { corner, initialEastward, initialWestward, initialNorthward, initialSouthward });

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
            Wall corner = new(ConsoleColor.Yellow, (cornerCol, 10));
            Bullet bullet = new((7, bulletRow), Direction.East);
            Bullet bulletAtEdge = bullet with { Location = (cornerCol, bulletRow) };
            Bullet bulletBeyondEdge = bullet with { Location = (cornerCol + 1, bulletRow) };
            Level level = new(Player.Default(1, 1), new HashSet<IDisplayable>() {bullet, corner});
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
        public void BulletDiesAtSouthEdgeOfLevel_Test()
        {
            // Arrange
            int cornerRow = 10;
            Wall corner = new(ConsoleColor.Yellow, (10, cornerRow));
            Bullet bullet = new((7, 2), Direction.South);
            Bullet bulletAtEdge = bullet with { Location = (7, 10) };
            Bullet bulletBeyondEdge = bullet with { Location = (7, 11) };
            Level level = new(Player.Default(1, 1), new HashSet<IDisplayable>() { bullet, corner });
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

    }
}
