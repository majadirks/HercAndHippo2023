using HercAndHippoLibCs;
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
            Level level = new(initial, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { wall, corner });

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
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { wall, corner });
            Bullet initialBullet = new Bullet((3, 2), Direction.East).ForgetId();
            Bullet bulletOverWall = new Bullet(wall.Location, Direction.East).ForgetId();

            // Act
            level = level.RefreshCyclables(ActionInput.ShootEast).ForgetIds();
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
            Level level = new(initial, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { bwall, corner });

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
            Level level = new(initial, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { bwall, corner });

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
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { door, corner });
            Bullet initialBullet = new Bullet((3, 2), Direction.East).ForgetId();
            Bullet bulletOverWall = new Bullet(door.Location, Direction.East).ForgetId();

            // Act
            level = level.RefreshCyclables(ActionInput.ShootEast).ForgetIds();
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
            Level level = new(initial, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { door, corner });

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
            Level level = new(initial, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { door, key, corner });

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

        [TestMethod]
        public void DoorYieldsToPlayerWithKey_IdentifiedByStringId_Test()
        {
            // Arrange
            Player initial = Player.Default((2, 2));

            Player playerAtDoor = Player.Default((4, 2));
            Door door = new(Color.Cyan, (4, 2));
            Key key = new(Color.Cyan, (3, 2));
            Player secondPosition = Player.Default((3, 2)) with { Inventory = new Inventory(starterItem: key) };
            Wall corner = new(Color.White, (10, 10));
            Level level = new(initial, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { door, key, corner });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(secondPosition.Location, level.Player.Location); // Player is adjacent to to door after moving east
            Assert.IsTrue(level.Player.Has<Key>(Color.Cyan));// Player has picked up the key
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreNotEqual(secondPosition.Location, level.Player.Location); // door has not blocked further eastward movement.
            Assert.AreEqual(playerAtDoor.Location, level.Player.Location); // Player has moved over door
            Assert.IsFalse(level.Contains(door)); // No more door!
            Assert.IsFalse(level.Player.Has<Key>(Color.Cyan)) ; // Player no longer has key
        }

        [TestMethod]
        public void MotionBlockedByDoor_IffNoKey_Test()
        {
            // Arrange
            Player player = new(location: (2, 2), health: 100, ammoCount: 0, inventory: Inventory.EmptyInventory);
            Door door = new(Color.Magenta, (2, 3));
            Key key = new(Color.Magenta, (4, 2));
            Wall corner = new(Color.Yellow, (10, 10));
            Level level = new(player: player, gravity: Gravity.None, secondaryObjects: new() { door, key, corner });

            // Act and assert: Move player around door, check IsBlocked() methods.
            // Initially north of wall
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.South));

            // Player moves to east of door, hence blocked west
            level = level.RefreshCyclables(ActionInput.MoveEast);
            level = level.RefreshCyclables(ActionInput.MoveSouth);
            player = level.Player;
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.West));

            // Player moves to south of door, hence blocked north
            level = level.RefreshCyclables(ActionInput.MoveSouth);
            level = level.RefreshCyclables(ActionInput.MoveWest);
            player = level.Player;
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.North));

            // Player moves to west of door, hence blocked east.
            level = level.RefreshCyclables(ActionInput.MoveWest);
            level = level.RefreshCyclables(ActionInput.MoveNorth);
            player = level.Player;
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.East));

            // Player picks up key
            level = level.RefreshCyclables(ActionInput.MoveNorth)
                .RefreshCyclables(ActionInput.MoveEast) // back to (2,2)
                .RefreshCyclables(ActionInput.MoveEast)
                .RefreshCyclables(ActionInput.MoveEast); // reach key
            player = level.Player;
            Assert.IsTrue(player.Has<Key>(Color.Magenta));

            level = level.RefreshCyclables(ActionInput.MoveWest)
                .RefreshCyclables(ActionInput.MoveWest); // back to (2,2)
            player = level.Player;

            // Player is north of door, but motion is not blocked south
            Assert.AreEqual((2, 2), player.Location);
            Assert.IsTrue(player.ObjectLocatedTo(level, Direction.South));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.South));

            // Player moves to east of door
            level = level.RefreshCyclables(ActionInput.MoveEast);
            level = level.RefreshCyclables(ActionInput.MoveSouth);
            player = level.Player;
            Assert.IsTrue(player.ObjectLocatedTo(level, Direction.West));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.West));

            // Player moves to south of door
            level = level.RefreshCyclables(ActionInput.MoveSouth);
            level = level.RefreshCyclables(ActionInput.MoveWest);
            player = level.Player;
            Assert.IsTrue(player.ObjectLocatedTo(level, Direction.North));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.North));

            // Player moves to west of door
            level = level.RefreshCyclables(ActionInput.MoveWest);
            level = level.RefreshCyclables(ActionInput.MoveNorth);
            player = level.Player;
            Assert.IsTrue(player.ObjectLocatedTo(level, Direction.East));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.East));
        }
    }
}
