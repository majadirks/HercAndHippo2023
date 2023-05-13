using static HercAndHippoLibCs.Inventory;
namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Player_Test
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
            // Move east until player reaches corner
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
        /// Check that two player objects are equal if they have equivalent inventories
        /// </summary>
        public void PlayerEquality_Test()
        {
            static Inventory GetNewInventory() => new(new HashSet<ITakeable>());
            // Arrange
            Inventory p1Inventory = GetNewInventory();
            Player player1 = new((2, 2), Health: 100, AmmoCount: 0, Inventory: p1Inventory);
            Inventory p2Inventory = GetNewInventory();
            Player player2 = new((2, 2), Health: 100, AmmoCount: 0, Inventory: p2Inventory);
            // Assert
            Assert.AreEqual(player1, player2);            
        }
    }
}
