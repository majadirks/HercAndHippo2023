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
    }
}