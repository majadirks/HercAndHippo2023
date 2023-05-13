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
        public void PlayerEquality()
        {
            // Arrange
            Player player1 = new((2, 2), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            Player player2 = new((2, 2), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            // Assert
            Assert.AreEqual(player1, player2);
        }
    }
}
