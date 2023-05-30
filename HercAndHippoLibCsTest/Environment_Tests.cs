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
            Wall wall = new(Color.White, (4, 2));
            Wall corner = new(Color.White, (10, 10));
            Level level = new(initial, new HashSet<HercAndHippoObj> { wall, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(secondPosition.Location, level.Player.Location); // Player is adjacent to to wall after moving east
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(secondPosition.Location, level.Player.Location); // Wall has blocked further eastward movement.
        }

        [TestMethod]
        public void WallStopsBullet_Test()
        {
            // Arrange
            Player player = Player.Default((2, 2)) with { AmmoCount = 1 };
            Wall wall = new(Color.White, (4, 2));
            Wall corner = new(Color.White, (10, 10));
            Level level = new(player, new HashSet<HercAndHippoObj> { wall, corner });
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
            BreakableWall bwall = new(Color.White, (4, 2));
            BreakableWall corner = new(Color.White, (10, 10));
            Level level = new(initial, new HashSet<HercAndHippoObj> { bwall, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(secondPosition.Location, level.Player.Location); // Player is adjacent to to wall after moving east
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(secondPosition.Location, level.Player.Location); // Wall has blocked further eastward movement.
        }

        [TestMethod]
        public void BreakableWallStopsBulletAndDies_Test()
        {
            // Arrange
            Player initial = Player.Default((2, 2)) with { AmmoCount = 1 };
            BreakableWall bwall = new(Color.White, (4, 2));
            Wall corner = new(Color.White, (10, 10));
            Level level = new(initial, new HashSet<HercAndHippoObj> { bwall, corner });

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
            Door door = new(Color.DarkMagenta, (4, 2));
            Wall corner = new(Color.White, (10, 10));
            Level level = new(player, new HashSet<HercAndHippoObj> { door, corner });
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
            Door door = new(Color.Cyan, (4, 2));
            Wall corner = new(Color.White, (10, 10));
            Level level = new(initial, new HashSet<HercAndHippoObj> { door, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(secondPosition.Location, level.Player.Location); // Player is adjacent to to door after moving east
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(secondPosition.Location, level.Player.Location); // door has blocked further eastward movement.
        }

        [TestMethod]
        public void DoorYieldsToPlayerWithKey_Test()
        {
            // Arrange
            Player initial = Player.Default((2, 2));

            Player playerAtDoor = Player.Default((4, 2));
            Door door = new(Color.Cyan, (4, 2));
            Key key = new(Color.Cyan, (3, 2));
            Player secondPosition = Player.Default((3, 2)) with { Inventory = new Inventory(starterItem: key) };
            Wall corner = new(Color.White, (10, 10));
            Level level = new(initial, new HashSet<HercAndHippoObj> { door, key, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(secondPosition.Location, level.Player.Location); // Player is adjacent to to door after moving east
            Assert.IsTrue(level.Player.Has<Key>(Color.Cyan));// Player has picked up the key
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreNotEqual(secondPosition.Location, level.Player.Location); // door has not blocked further eastward movement.
            Assert.AreEqual(playerAtDoor.Location, level.Player.Location); // Player has moved over door
            Assert.IsFalse(level.Contains(door)); // No more door!
            Assert.IsFalse(level.Player.Has<Key>(Color.Cyan)); // Player no longer has key
        }
    }
}
