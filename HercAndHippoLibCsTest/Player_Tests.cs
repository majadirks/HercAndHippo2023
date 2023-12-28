using System.Runtime.CompilerServices;
using static HercAndHippoLibCs.Inventory;
namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Player_Tests
    {
        [TestMethod]
        public void PlayerMovesFasterAfterAccelerating_HeadingEast_Test()
        {
            // Arrange
            Player player = Player.Default((2, 2));
            Noninteractor edge = new((1000, 10));
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { edge });
            Velocity initialVelocity = player.Velocity;

            // Act
            for (int i = 0; i < 10; i++)
            {
                level = level.RefreshCyclables(ActionInput.MoveEast);
            }

            // Assert: Velocity has increased
            Assert.IsTrue(Math.Abs(level.Player.Velocity) > Math.Abs(initialVelocity) && level.Player.Velocity > 0);

            // Arrange again
            Column curCol = level.Player.Location.Col;
            // Act again
            level = level.RefreshCyclables(ActionInput.MoveEast);
            // Assert: Player has moved more than one column
            Assert.IsTrue(level.Player.Location.Col - curCol > 1);
        }

        [TestMethod]
        public void PlayerMovesFasterAfterAccelerating_HeadingWest_Test()
        {
            // Arrange
            Player player = Player.Default((999, 2));
            Noninteractor edge = new((1000, 10));
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { edge });
            Velocity initialVelocity = player.Velocity;

            // Act
            for (int i = 0; i < 10; i++)
            {
                level = level.RefreshCyclables(ActionInput.MoveWest);
            }

            // Assert: Velocity has increased westward
            Assert.IsTrue(Math.Abs(level.Player.Velocity) > Math.Abs(initialVelocity) && level.Player.Velocity < 0);

            // Arrange again
            Column curCol = level.Player.Location.Col;
            // Act again
            level = level.RefreshCyclables(ActionInput.MoveWest);
            // Assert: Player has moved more than one column
            Assert.IsTrue(curCol - level.Player.Location.Col > 1);
        }

        [TestMethod]
        public void PlayerCanShootWhileVelocityIsNonzero_Test()
        {
            // Arrange
            Player player = new((5, 5), 100, 100, EmptyInventory);
            ShotCounter counter = new((10, 5), 0);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { counter });

            // Act
            level = level.RefreshCyclables(ActionInput.MoveEast)
                .RefreshCyclables(ActionInput.MoveEast)
                .RefreshCyclables(ActionInput.MoveEast)
                .RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Player.Velocity > 0);
            level = level.RefreshCyclables(ActionInput.ShootEast);

            // Assert
            Assert.IsTrue(level.Player.Velocity > 0);
            Assert.IsTrue(level.LevelObjects.Where(b => b is Bullet).Any());

        }

        [TestMethod]
        public void PlayerCanMoveWithinBounds_Test()
        {
            // Arrange
            Player initial = new(location: (5, 5), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Player eastPlayer = initial with { Location = (6, 5) };
            Player northeastPlayer = initial with { Location = (6, 4) };
            Player northPlayer = initial with { Location = (5, 4) };
            Player southPlayer = initial with { Location = (5, 6) };

            Level level = new(player: initial, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>()
            {
                new Wall(Color.DarkRed, (10, 10))
            });

            // Move east and check
            Assert.IsTrue(level.Contains(initial));
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.AreEqual(eastPlayer.Location, level.Player.Location);

            // Move north from there; now northeast of initial
            level = level.RefreshCyclables(ActionInput.MoveNorth);
            Assert.AreEqual(northeastPlayer.Location, level.Player.Location);

            // Move west from there; now north of initial
            level = level.RefreshCyclables(ActionInput.MoveWest);
            Assert.AreEqual(northPlayer.Location, level.Player.Location);

            // Move south twice; player is now south of initial
            level = level.RefreshCyclables(ActionInput.MoveSouth).RefreshCyclables(ActionInput.MoveSouth);
            Assert.AreEqual(southPlayer.Location, level.Player.Location);
        }

        [TestMethod]
        public void PlayerCannotMoveWestOfMinCol_Test()
        {
            // Arrange
            Player player = new((Column.MIN_COL + 5, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Wall corner = new(Color.Yellow, (10, 10));
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { corner });
            int attempt = 1;
            int maxAttempt = 10;
            Assert.IsFalse(level.Player.Location.Col == Column.MIN_COL);

            // Act
            // Move west until we reach min col
            while (level.Player.Location.Col > Column.MIN_COL)
            {
                if (attempt > maxAttempt) Assert.IsTrue(false);
                attempt++;
                level = level.RefreshCyclables(ActionInput.MoveWest);
            }
            Assert.IsTrue(level.Player.Location.Col == Column.MIN_COL);
            // Move west once more
            level = level.RefreshCyclables(ActionInput.MoveWest);

            // Assert: Player still in the same column (did not move further west)
            Assert.IsTrue(level.Player.Location.Col == Column.MIN_COL);
        }

        [TestMethod]
        public void PlayerCannotMoveNorthOfMinRow_Test()
        {
            // Arrange
            Player player = new((5, Row.MIN_ROW + 5), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Wall corner = new(Color.Yellow, (10, 10));
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { corner });
            int attempt = 1;
            int maxAttempt = 10;
            Assert.IsFalse(level.Player.Location.Row == Row.MIN_ROW);

            // Act
            // Move north until we reach min row
            while (level.Player.Location.Row > Row.MIN_ROW)
            {
                if (attempt > maxAttempt) Assert.IsTrue(false);
                attempt++;
                level = level.RefreshCyclables(ActionInput.MoveNorth);
            }
            Assert.IsTrue(level.Player.Location.Row == Row.MIN_ROW);
            // Move north once more
            level = level.RefreshCyclables(ActionInput.MoveNorth);

            // Assert: Player still in the same column (did not move further west)
            Assert.IsTrue(level.Player.Location.Row == Row.MIN_ROW);
        }

        [TestMethod]
        public void PlayerCannotMoveEastOfCorner_Test()
        {
            // Arrange
            Player player = new((2, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Wall corner = new(Color.Yellow, (10, 10));
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { corner });
            int attempt = 1;
            int maxAttempt = 10;
            Assert.IsFalse(level.Player.Location.Col == corner.Location.Col);

            // Act
            // Move east until player reaches corner
            while (level.Player.Location.Col < corner.Location.Col)
            {
                if (attempt > maxAttempt) Assert.IsTrue(false);
                attempt++;
                level = level.RefreshCyclables(ActionInput.MoveEast);
            }
            Assert.IsTrue(level.Player.Location.Col == corner.Location.Col);
            // Move east once more
            level = level.RefreshCyclables(ActionInput.MoveEast);

            // Assert: Player still in the same column (did not move further east)
            Assert.IsTrue(level.Player.Location.Col == corner.Location.Col);
        }

        [TestMethod]
        public void PlayerCannotMoveSouthOfCorner_Test()
        {
            // Arrange
            Player player = new((2, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Wall corner = new(Color.Yellow, (10, 10));
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj> { corner });
            int attempt = 1;
            int maxAttempt = 10;
            Assert.IsFalse(level.Player.Location.Row == corner.Location.Row);

            // Act
            // Move south until player reaches corner
            while (level.Player.Location.Row < corner.Location.Row)
            {
                if (attempt > maxAttempt) Assert.IsTrue(false);
                attempt++;
                level = level.RefreshCyclables(ActionInput.MoveSouth);
            }
            Assert.IsTrue(level.Player.Location.Row == corner.Location.Row);
            // Move east once more
            level = level.RefreshCyclables(ActionInput.MoveSouth);

            // Assert: Player still in the same row (did not move further south)
            Assert.IsTrue(level.Player.Location.Row == corner.Location.Row);
        }

        [TestMethod]
        /// <summary>
        /// Check that two player objects are equal if they have both have empty inventories
        /// </summary>
        public void PlayerEqualityWithEmptyInventory_Test()
        {
            static Inventory GetNewInventory() => new(new HashSet<ITakeable>());
            // Arrange
            Inventory p1Inventory = GetNewInventory();
            Player player1 = new((2, 2), health: 100, ammoCount: 0, inventory: p1Inventory);
            Inventory p2Inventory = GetNewInventory();
            Player player2 = new((2, 2), health: 100, ammoCount: 0, inventory: p2Inventory);
            // Assert
            Assert.AreEqual(player1.Inventory.GetHashCode(), player2.Inventory.GetHashCode());
            Assert.AreEqual(player1, player2);
        }

        [TestMethod]
        /// <summary>
        /// Check that two player objects are equal if they have both have empty inventories
        /// </summary>
        public void PlayerEqualityWithNonemptyInventory_Test()
        {
            static Key GetNewKey() => new(Color.DarkBlue, (3, 2));
            static Inventory GetNewInventory() => new Inventory(new HashSet<ITakeable>()).AddItem(GetNewKey());
            // Arrange
            Inventory p1Inventory = GetNewInventory();
            Player player1 = new((2, 2), health: 100, ammoCount: 0, inventory: p1Inventory);
            Inventory p2Inventory = GetNewInventory();
            Player player2 = new((2, 2), health: 100, ammoCount: 0, inventory: p2Inventory);
            // Assert
            Assert.AreEqual(player1, player2);
            Assert.AreEqual(player1.Inventory.GetHashCode(), player2.Inventory.GetHashCode());
            Assert.AreEqual(player1.GetHashCode(), player2.GetHashCode());
        }

        [TestMethod]
        public void EmptyInventoriesAreEqual_Test()
        {
            // Arrange
            static Inventory GetNewInventory() => new(new HashSet<ITakeable>());
            Inventory inv1 = GetNewInventory();
            Inventory inv2 = GetNewInventory();
            // Assert
            Assert.AreEqual(inv1.GetHashCode(), inv2.GetHashCode());
            Assert.AreEqual(inv1, inv2);
        }

        [TestMethod]
        public void NonemptyInventoriesAreEqual_Test()
        {
            // Arrange
            static Inventory GetNewInventory() => new(new HashSet<ITakeable>());
            static Key GetNewKey() => new(Color.Cyan, (5, 5));
            Inventory inv1 = GetNewInventory().AddItem(GetNewKey());
            Inventory inv2 = GetNewInventory();
            Assert.AreNotEqual(inv1.GetHashCode(), inv2.GetHashCode());
            Assert.AreNotEqual(inv1, inv2);
            inv2 = inv2.AddItem(GetNewKey());
            // Assert
            Assert.AreEqual(inv1.GetHashCode(), inv2.GetHashCode());
            Assert.AreEqual(inv1, inv2);
        }

        [TestMethod]
        public void NonemptyInventoriesAreEqual_InDifferentOrders_Test()
        {
            // Arrange
            static Inventory GetNewInventory() => new(new HashSet<ITakeable>());
            static Key GetNewCyanKey() => new(Color.Cyan, (5, 5));
            static Key GetNewMagentaKey() => new(Color.Magenta, (5, 5));
            Inventory inv1 = GetNewInventory().AddItem(GetNewCyanKey()).AddItem(GetNewMagentaKey());
            Inventory inv2 = GetNewInventory();
            Assert.AreNotEqual(inv1.GetHashCode(), inv2.GetHashCode());
            Assert.AreNotEqual(inv1, inv2);
            inv2 = inv2.AddItem(GetNewMagentaKey()).AddItem(GetNewCyanKey()); // NB keys added in opposite order
            // Assert
            Assert.AreEqual(inv1.GetHashCode(), inv2.GetHashCode());
            Assert.AreEqual(inv1, inv2);
        }

        [TestMethod]
        public void InventoriesWithDifferentItemsAreNotEqual_Test()
        {
            // Arrange
            Inventory inv1 = new(new Key(Color.Cyan, (5, 5)));
            Inventory inv2 = new(new Key(Color.Blue, (5, 5)));
            // Assert
            Assert.AreNotEqual(inv1.GetHashCode(), inv2.GetHashCode());
            Assert.AreNotEqual(inv1, inv2);
        }

        [TestMethod]
        public void OnTouchMethodTriggeredWhenPlayerTouchesAnITouchable_Test()
        {
            // Arrange
            int startCount = 0;
            Player player = new((2, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
            TouchCounter initialCounter = new((3, 2), startCount);
            TouchCounter cycledCounter = new((3, 2), startCount + 1);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { initialCounter });

            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));
            Assert.AreEqual(player.Location, level.Player.Location);

            // Act: player attempts to move east, but is blocked by counter, which increments
            level = level.RefreshCyclables(ActionInput.MoveEast);

            // Assert
            // Check that counter has incremented
            Assert.IsFalse(level.Contains(initialCounter));
            Assert.IsTrue(level.Contains(cycledCounter));
            // Check that player has not moved
            Assert.AreEqual(player.Location, level.Player.Location);
        }

        [TestMethod]
        public void PlayerCanShootInAllDirections_Test()
        {
            // Arrange
            Player player = new((5, 5), health: 100, ammoCount: 5, inventory: EmptyInventory);
            Bullet northBullet = new((5, 4), Direction.North);
            Bullet eastBullet = new((6, 5), Direction.East);
            Bullet southBullet = new((5, 6), Direction.South);
            Bullet westBullet = new((4, 5), Direction.West);
            Wall corner = new(Color.Yellow, (10, 10));
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { corner });

            Assert.IsFalse(level.Contains(northBullet));
            Assert.IsFalse(level.Contains(eastBullet));
            Assert.IsFalse(level.Contains(southBullet));
            Assert.IsFalse(level.Contains(westBullet));

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.ShootEast);
            Assert.IsTrue(level.Contains(eastBullet));
            level = level.RefreshCyclables(ActionInput.ShootWest);
            Assert.IsTrue(level.Contains(westBullet));
            level = level.RefreshCyclables(ActionInput.ShootNorth);
            Assert.IsTrue(level.Contains(northBullet));
            level = level.RefreshCyclables(ActionInput.ShootSouth);
            Assert.IsTrue(level.Contains(southBullet));
        }

        [TestMethod]
        public void PlayerCanShootWhenOnBoundary_Test()
        {
            // Arrange
            Bullet westBullet = new((9, 9), Direction.West);
            Wall corner = new(Color.Yellow, (10, 10));
            Player player = new((10, 9), health: 100, ammoCount: 5, inventory: EmptyInventory);
            Level level = new(player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { corner });
            Assert.IsFalse(level.Contains(westBullet));

            // Act
            level = level.RefreshCyclables(ActionInput.ShootWest);
            // Assert
            Assert.IsTrue(level.Contains(westBullet));

            // Arrange
            level = level.WithPlayer(player with { Location = (9, 10) }); // west of corner
            Bullet northBullet = new((9, 9), Direction.North);
            // Act
            level = level.RefreshCyclables(ActionInput.ShootNorth);
            //Assert
            Assert.IsTrue(level.Contains(northBullet));

        }

        [TestMethod]
        public void PlayerCanPickUpKeyByMoving_Test()
        {
            // Arrange: Player west of key
            Player player = new((2, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Color keyColor = Color.DarkRed;
            Key key = new(keyColor, (3, 2));
            Player movedPlayer = player with { Location = key.Location, Inventory = player.Inventory.AddItem(key) };
            Level level = new(player: player, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { key });

            Assert.IsTrue(level.Contains(player));
            Assert.IsTrue(level.Contains(key));
            Assert.IsFalse(level.Player.Has<Key>(keyColor));
            Assert.IsFalse(level.Contains(movedPlayer));

            // Act: Move east
            level = level.RefreshCyclables(ActionInput.MoveEast);

            // Assert: Player has moved and has key in inventory. Key not in level.
            Assert.IsFalse(level.Contains(player));
            Assert.IsFalse(level.Contains(key));
            Assert.IsTrue(level.Player.Has<Key>(keyColor));

            Assert.AreEqual(movedPlayer.Inventory, level.Player.Inventory);
            Assert.AreEqual(movedPlayer.Location, level.Player.Location);
        }

        [TestMethod]
        public void Take_Test()
        {
            // Arrange
            Player player = new((2, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Color keyColor = Color.Magenta;
            Key key = new(keyColor, (3, 2));
            Assert.IsFalse(player.Inventory.Contains(key));
            Assert.IsFalse(player.Has<Key>(keyColor));

            // Act
            player = player.Take(key);

            // Assert
            Assert.IsTrue(player.Inventory.Contains(key));
            Assert.IsTrue(player.Has<Key>(keyColor));
        }

        [TestMethod]
        public void Has_Test()
        {
            // Arrange
            Color keyColor = Color.Magenta;
            Key key = new(keyColor, (3, 2));
            Player player = new((2, 2), health: 100, ammoCount: 0, inventory: new(key));
            // Assert
            Assert.IsTrue(player.Has<Key>(keyColor));
            Assert.IsFalse(player.Has<Key>(Color.Cyan));
        }

        [TestMethod]
        public void DropItem_Test()
        {
            // Arrange
            Color keyColor = Color.Magenta;
            Key key = new(keyColor, (3, 2));
            Player player = new((2, 2), health: 100, ammoCount: 0, inventory: new(key));
            Assert.IsTrue(player.Has<Key>(keyColor));

            // Act
            (bool dropped, ITakeable? droppedkey, player) = player.DropItem<Key>(keyColor);

            // Assert
            Assert.IsTrue(dropped);
            Assert.AreEqual(droppedkey, key);
            Assert.IsFalse(player.Has<Key>(keyColor));
        }

        [TestMethod]
        public void CannotDropItemNotHeld_Test()
        {
            // Arrange
            Color keyColor = Color.Magenta;
            Player player = new((2, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Assert.IsFalse(player.Has<Key>(keyColor));

            // Act
            (bool dropped, ITakeable? droppedkey, player) = player.DropItem<Key>(keyColor);

            // Assert
            Assert.IsFalse(dropped);
            Assert.AreEqual(default, droppedkey);
            Assert.IsFalse(player.Has<Key>(keyColor));
        }

        [TestMethod]
        public void MotionBlockedByWall_Test()
        {
            // Arrange
            Player player = new(location: (2, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Wall wall = new(Color.Magenta, (2, 3));
            Wall corner = new(Color.Yellow, (10, 10));
            Level level = new(player: player, gravity: Gravity.None, secondaryObjects: new() { wall, corner });

            // Act and assert: Move player around wall, check IsBlocked() methods.
            // Initially north of wall
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.South));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.North));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.East));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.West));

            // Player moves to east of wall, hence blocked west
            level = level.RefreshCyclables(ActionInput.MoveEast);
            level = level.RefreshCyclables(ActionInput.MoveSouth);
            player = level.Player;
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.South));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.North));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.East));
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.West));

            // Player moves to south of wall, hence blocked north
            level = level.RefreshCyclables(ActionInput.MoveSouth);
            level = level.RefreshCyclables(ActionInput.MoveWest);
            player = level.Player;
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.South));
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.North));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.East));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.West));


            // Player moves to west of wall, hence blocked east.
            // The player is also blocked west because they are located in the first column.
            level = level.RefreshCyclables(ActionInput.MoveWest);
            level = level.RefreshCyclables(ActionInput.MoveNorth);
            player = level.Player;
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.South));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.North));
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.East));
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.West));
        }

        [TestMethod]
        public void MotionNotBlockedByAmmo_Test()
        {
            // Arrange
            Player player = new(location: (2, 2), health: 100, ammoCount: 0, inventory: EmptyInventory);
           
            Ammo ammo = new((2, 3), Count: 5);
            Wall corner = new(Color.Yellow, (10, 10));
            Level level = new(player: player, gravity: Gravity.None, secondaryObjects: new() {ammo, corner });

            // Act and assert: Move player around ammo, check IsBlocked() methods.
            // Initially north of wall
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.South));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.North));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.East));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.West));

            // Player moves to east of ammo
            level = level.RefreshCyclables(ActionInput.MoveEast);
            level = level.RefreshCyclables(ActionInput.MoveSouth);
            player = level.Player;
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.South));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.North));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.East));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.West));

            // Player moves to south of ammo
            level = level.RefreshCyclables(ActionInput.MoveSouth);
            level = level.RefreshCyclables(ActionInput.MoveWest);
            player = level.Player;
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.South));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.North));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.East));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.West));


            // Player moves to west of ammo
            // The player is blocked west because they are located in the first column.
            level = level.RefreshCyclables(ActionInput.MoveWest);
            level = level.RefreshCyclables(ActionInput.MoveNorth);
            player = level.Player;
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.South));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.North));
            Assert.IsFalse(player.MotionBlockedTo(level, Direction.East));
            Assert.IsTrue(player.MotionBlockedTo(level, Direction.West));
        }

    }
}
