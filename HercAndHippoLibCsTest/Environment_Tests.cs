using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Environment_Tests
    {

        [TestMethod]
        public void WallStopsPlayer_Test()
        {
            // Arrange
            Player initial = Player.Default((2, 2));
            Player secondPosition = Player.Default((3, 2));
            Player impossiblePosition = Player.Default((4, 2));
            Wall wall = new(ConsoleColor.White, (4, 2));
            Wall corner = new(ConsoleColor.White, (10, 10));
            Level level = new(initial, new HashSet<ILocatable> { wall, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // Player is adjacent to to wall after moving east
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // Wall has blocked further eastward movement.
            Assert.IsFalse(level.Contains(impossiblePosition));
        }

        [TestMethod]
        public void WallStopsBullet_Test()
        {
            // Arrange
            Player player = Player.Default((2, 2)) with { AmmoCount = 1 };
            Wall wall = new(ConsoleColor.White, (4, 2));
            Wall corner = new(ConsoleColor.White, (10, 10));
            Level level = new(player, new HashSet<ILocatable> { wall, corner });
            Bullet initialBullet = new((3, 2), Direction.East);
            Bullet bulletOverWall = new(wall.Location, Direction.East);

            // Act
            level = level.RefreshCyclables(ActionInput.ShootEast);
            Assert.IsTrue(level.Contains(initialBullet)); // Bullet is between player and wall
            Assert.IsFalse(level.Contains(bulletOverWall));
            // On next cycle, bullet moves to overlap wall. On subsequent cycle, bullet is gone.
            level = level.RefreshCyclables(ActionInput.NoAction).RefreshCyclables(ActionInput.NoAction);
            
            // Assert
            Assert.IsFalse(level.LevelObjects.Where(b => b is Bullet).Any());
        }

        [TestMethod]
        public void BreakableWallStopsPlayer_Test()
        {
            // Arrange
            Player initial = Player.Default((2, 2));
            Player secondPosition = Player.Default((3, 2));
            Player impossiblePosition = Player.Default((4, 2));
            BreakableWall bwall = new(ConsoleColor.White, (4, 2));
            BreakableWall corner = new(ConsoleColor.White, (10, 10));
            Level level = new(initial, new HashSet<ILocatable> { bwall, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // Player is adjacent to to wall after moving east
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // Wall has blocked further eastward movement.
            Assert.IsFalse(level.Contains(impossiblePosition));
        }

        [TestMethod]
        public void BreakableWallStopsBulletAndDies_Test()
        {
            // Arrange
            Player initial = Player.Default((2, 2)) with { AmmoCount = 1 };
            BreakableWall bwall = new(ConsoleColor.White, (4, 2));
            Wall corner = new(ConsoleColor.White, (10, 10));
            Level level = new(initial, new HashSet<ILocatable> { bwall, corner });

            // Act
            level = level.RefreshCyclables(ActionInput.ShootEast);
            Assert.IsTrue(level.LevelObjects.Where(b => b is Bullet).Count() == 1);
            level = level.RefreshCyclables(ActionInput.NoAction).RefreshCyclables(ActionInput.NoAction);

            // Assert
            Assert.IsFalse(level.LevelObjects.Where(bw => bw is BreakableWall).Any()); 
            Assert.IsFalse(level.LevelObjects.Where(b => b is Bullet).Any());
        }

        [TestMethod]
        public void DoorStopsBullet_Test()
        {
            // Arrange
            Player player = Player.Default((2, 2)) with { AmmoCount = 1 };
            Door door = new(ConsoleColor.DarkMagenta, (4, 2));
            Wall corner = new(ConsoleColor.White, (10, 10));
            Level level = new(player, new HashSet<ILocatable> { door, corner });
            Bullet initialBullet = new((3, 2), Direction.East);
            Bullet bulletOverWall = new(door.Location, Direction.East);

            // Act
            level = level.RefreshCyclables(ActionInput.ShootEast);
            Assert.IsTrue(level.Contains(initialBullet)); // Bullet is between player and wall
            Assert.IsFalse(level.Contains(bulletOverWall));
            // On next cycle, bullet moves to overlap wall. On subsequent cycle, bullet is gone.
            level = level.RefreshCyclables(ActionInput.NoAction).RefreshCyclables(ActionInput.NoAction);

            // Assert
            Assert.IsFalse(level.LevelObjects.Where(b => b is Bullet).Any());
        }


        [TestMethod]
        public void DoorStopsPlayerWithoutKey_Test()
        {
            // Arrange
            Player initial = Player.Default((2, 2));
            Player secondPosition = Player.Default((3, 2));
            Player playerAtDoor = Player.Default((4, 2));
            Door door = new(ConsoleColor.Cyan, (4, 2));
            Wall corner = new(ConsoleColor.White, (10, 10));
            Level level = new(initial, new HashSet<ILocatable> { door, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // Player is adjacent to to door after moving east
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // door has blocked further eastward movement.
            Assert.IsFalse(level.Contains(playerAtDoor));
        }

        [TestMethod]
        public void DoorYieldsToPlayerWithKey_Test()
        {
            // Arrange
            Player initial = Player.Default((2, 2));
           
            Player playerAtDoor = Player.Default((4, 2));
            Door door = new(ConsoleColor.Cyan, (4, 2));
            Key key = new(ConsoleColor.Cyan, (3, 2));
            Player secondPosition = Player.Default((3, 2)) with { Inventory = new Inventory(starterItem: key) };
            Wall corner = new(ConsoleColor.White, (10, 10));
            Level level = new(initial, new HashSet<ILocatable> { door, key, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // Player is adjacent to to door after moving east
            Assert.IsTrue(level.Player.Has<Key>(ConsoleColor.Cyan));// Player has picked up the key
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsFalse(level.Contains(secondPosition)); // door has not blocked further eastward movement.
            Assert.IsTrue(level.Contains(playerAtDoor)); // Player has moved over door
            Assert.IsFalse(level.Contains(door)); // No more door!
            Assert.IsFalse(level.Player.Has<Key>(ConsoleColor.Cyan)); // Player no longer has key
        }
    }
}
