using static HercAndHippoLibCs.Inventory;
namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Player_Tests
    {
        [TestMethod]
        public void PlayerCanMoveWithinBounds_Test()
        {
            // Arrange
            Player initial = new(Location:(5, 5), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            Player eastPlayer = initial with { Location = (6, 5) };
            Player northeastPlayer = initial with { Location = (6, 4) };
            Player northPlayer = initial with { Location = (5, 4) };
            Player southPlayer = initial with { Location = (5, 6) };

            Level level = new(player: initial, displayables: new HashSet<IDisplayable>()
            {
                new Wall(ConsoleColor.DarkRed, (10, 10))
            });

            // Move east and check
            Assert.IsTrue(level.Contains(initial));
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsFalse(level.Contains(initial));
            Assert.IsTrue(level.Contains(eastPlayer));

            // Move north from there; now northeast of initial
            level = level.RefreshCyclables(ActionInput.MoveNorth);
            Assert.IsFalse(level.Contains(eastPlayer));
            Assert.IsTrue(level.Contains(northeastPlayer));

            // Move west from there; now north of initial
            level = level.RefreshCyclables(ActionInput.MoveWest);
            Assert.IsFalse(level.Contains(northeastPlayer));
            Assert.IsTrue(level.Contains(northPlayer));

            // Move south twice; player is now south of initial
            level = level.RefreshCyclables(ActionInput.MoveSouth).RefreshCyclables(ActionInput.MoveSouth);
            Assert.IsFalse(level.Contains(northPlayer));
            Assert.IsFalse(level.Contains(initial));
            Assert.IsTrue(level.Contains(southPlayer));
        }

        [TestMethod]
        public void PlayerCannotMoveWestOfMinCol_Test()
        {
            // Arrange
            Player player = new((Column.MIN_COL + 5, 2), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            Wall corner = new(ConsoleColor.Yellow, (10,10));
            Level level = new(player, new HashSet<IDisplayable> {corner});
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
            Player player = new((5, Row.MIN_ROW + 5), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            Wall corner = new(ConsoleColor.Yellow, (10, 10));
            Level level = new(player, new HashSet<IDisplayable> { corner });
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
            Player player = new((2, 2), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            Wall corner = new(ConsoleColor.Yellow, (10, 10));
            Level level = new(player, new HashSet<IDisplayable> { corner });
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
            Player player = new((2, 2), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            Wall corner = new(ConsoleColor.Yellow, (10, 10));
            Level level = new(player, new HashSet<IDisplayable> { corner });
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
            Player player1 = new((2, 2), Health: 100, AmmoCount: 0, Inventory: p1Inventory);
            Inventory p2Inventory = GetNewInventory();
            Player player2 = new((2, 2), Health: 100, AmmoCount: 0, Inventory: p2Inventory);
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
            static Key GetNewKey() => new(ConsoleColor.DarkBlue, (3, 2));
            static Inventory GetNewInventory() => new Inventory(new HashSet<ITakeable>()).AddItem(GetNewKey());
            // Arrange
            Inventory p1Inventory = GetNewInventory();
            Player player1 = new((2, 2), Health: 100, AmmoCount: 0, Inventory: p1Inventory);
            Inventory p2Inventory = GetNewInventory();
            Player player2 = new((2, 2), Health: 100, AmmoCount: 0, Inventory: p2Inventory);
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
            static Key GetNewKey() => new(ConsoleColor.Cyan, (5, 5));
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
        public void InventoriesWithDifferentItemsAreNotEqual_Test()
        {
            // Arrange
            Inventory inv1 = new(new Key(ConsoleColor.Cyan, (5,5)));
            Inventory inv2 = new(new Key(ConsoleColor.Blue, (5, 5)));
            // Assert
            Assert.AreNotEqual(inv1.GetHashCode(), inv2.GetHashCode());
            Assert.AreNotEqual(inv1, inv2);
        }

        [TestMethod]
        public void OnTouchMethodTriggeredWhenPlayerTouchesAnITouchable_Test()
        {
            // Arrange
            int startCount = 0;
            Player player = new((2, 2), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            Player movedPlayer = player with { Location = (3, 2) };
            TouchCounter initialCounter = new((3, 2), ConsoleColor.Green, startCount);
            TouchCounter cycledCounter = new((3, 2), ConsoleColor.Green, startCount + 1);
            Level level = new(player, displayables: new HashSet<IDisplayable>() { initialCounter });

            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));
            Assert.IsTrue(level.Contains(player));
            Assert.IsFalse(level.Contains(movedPlayer));

            // Act: player attempts to move east, but is blocked by counter, which increments
            level = level.RefreshCyclables(ActionInput.MoveEast);

            // Assert
            // Check that counter has incremented
            Assert.IsFalse(level.Contains(initialCounter));
            Assert.IsTrue(level.Contains(cycledCounter));
            // Check that player has not moved
            Assert.IsTrue(level.Contains(player));
            Assert.IsFalse(level.Contains(movedPlayer));
        }

        [TestMethod]
        public void PlayerCanShootInAllDirections_Test()
        {
            // Arrange
            Player player = new((5,5), Health: 100, AmmoCount: 5, Inventory: EmptyInventory);
            Bullet northBullet = new((5, 4), Direction.North);
            Bullet eastBullet = new((6, 5), Direction.East);
            Bullet southBullet = new((5,6), Direction.South);
            Bullet westBullet = new((4,5), Direction.West);
            Wall corner = new(ConsoleColor.Yellow, (10, 10));
            Level level = new(player, new HashSet<IDisplayable>() { corner });

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
            Wall corner = new(ConsoleColor.Yellow, (10, 10));
            Player player = new((10,9), Health: 100, AmmoCount: 5, Inventory: EmptyInventory);
            Level level = new(player, new HashSet<IDisplayable>() { corner });
            Assert.IsFalse(level.Contains(westBullet));

            // Act
            level = level.RefreshCyclables(ActionInput.ShootWest);
            // Assert
            Assert.IsTrue(level.Contains(westBullet));

            // Arrange
            level = level.WithPlayer(player with {Location = (9, 10)}); // west of corner
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
            Player player = new((2,2), Health: 100, AmmoCount: 0, Inventory:EmptyInventory);
            ConsoleColor keyColor = ConsoleColor.DarkRed;
            Key key = new(keyColor, (3, 2));
            Player movedPlayer = player with { Location = key.Location, Inventory = player.Inventory.AddItem(key) };
            Level level = new(player: player, displayables: new HashSet<IDisplayable>() { key });

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
            Assert.AreEqual(movedPlayer, level.Player);
            Assert.IsTrue(level.Contains(movedPlayer));
        }

        [TestMethod]
        public void Take_Test()
        {
            // Arrange
            Player player = new((2, 2), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            ConsoleColor keyColor = ConsoleColor.Magenta;
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
            ConsoleColor keyColor = ConsoleColor.Magenta;
            Key key = new(keyColor, (3, 2));
            Player player = new((2, 2), Health: 100, AmmoCount: 0, Inventory: new(key));
            // Assert
            Assert.IsTrue(player.Has<Key>(keyColor));
            Assert.IsFalse(player.Has<Key>(ConsoleColor.Cyan));
        }

        [TestMethod]
        public void DropItem_Test()
        {
            // Arrange
            ConsoleColor keyColor = ConsoleColor.Magenta;
            Key key = new(keyColor, (3, 2));
            Player player = new((2, 2), Health: 100, AmmoCount: 0, Inventory: new(key));
            Assert.IsTrue(player.Has<Key>(keyColor));

            // Act
            (ITakeable dropped, player) = player.DropItem<Key>(keyColor);

            // Assert
            Assert.AreEqual(dropped, key);
            Assert.IsFalse(player.Has<Key>(keyColor));
        }
    }
}
