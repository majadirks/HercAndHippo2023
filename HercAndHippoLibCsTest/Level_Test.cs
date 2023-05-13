using static HercAndHippoLibCs.Inventory;

namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Level_Test
    {
        [TestMethod]
        public void HeightAndWidth_Test()
        {
            // Arrange
            int expectedWidth = 4;
            int expectedHeight = 7;
            Player player = new((1, 1), Health: 100, AmmoCount: 0, Inventory: EmptyInventory);
            Level level = new(player: player, displayables: new HashSet<IDisplayable>()
            {
                new Wall(ConsoleColor.Yellow, new Location(Col: expectedWidth, Row: expectedHeight))
            }) ;

            // Assert
            Assert.AreEqual(expectedHeight, level.Height);
            Assert.AreEqual(expectedWidth, level.Width);
        }

        [TestMethod]
        public void HeightAndWidthDoNotChange_Test()
        {
            // Arrange
            int expectedWidth = 4;
            int expectedHeight = 7;
            Player player = new(new Location(Col: expectedWidth, Row: expectedHeight), Health: 100, AmmoCount: 5, Inventory: EmptyInventory);
            Level level = new(player: player, displayables: new HashSet<IDisplayable>()
            {
                new Wall(ConsoleColor.Yellow, new Location(Col: expectedWidth, Row: expectedHeight))
            });
            Bullet bulletEastOfBoundary = new((expectedWidth + 10, expectedHeight), Direction.East);
            Bullet bulletSouthOfBoundary = new(Location: (expectedWidth, expectedHeight + 1), Direction.South);

            // Act
            level = level.AddObject(bulletEastOfBoundary).AddObject(bulletSouthOfBoundary);

            // Assert: There is a bullet east of the bounds of the level, but width has not changed.
            Assert.IsTrue(level.LevelObjects.Contains(bulletEastOfBoundary));
            Assert.IsTrue(bulletEastOfBoundary.Location.Col > expectedWidth);
            Assert.IsTrue(bulletEastOfBoundary.Location.Col > level.Width);
            Assert.AreEqual(expectedWidth, level.Width);

            // Assert: There is a bullet south of the bounds of the level, but height has not changed
            Assert.IsTrue(level.LevelObjects.Contains(bulletSouthOfBoundary));
            Assert.IsTrue(bulletSouthOfBoundary.Location.Row > expectedHeight);
            Assert.IsTrue(bulletSouthOfBoundary.Location.Row > level.Height);
            Assert.AreEqual(expectedHeight, level.Height);
        }


        [TestMethod]
        public void RefreshCyclables_Test()
        {
            // Arrange
            Player player = new(new Location(Col: 1, Row: 1), Health: 100, AmmoCount: 5, Inventory: EmptyInventory);
            int startCount = 0;
            CycleCounter initialCounter = new((2, 2), ConsoleColor.Green, startCount);
            CycleCounter cycledCounter = new((2, 2), ConsoleColor.Green, startCount + 1);
            Level level = new(player: player, displayables: new HashSet<IDisplayable>() { initialCounter });

            // Check that we set this up correctly
            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));

            // Act
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert
            Assert.IsFalse(level.Contains(initialCounter));
            Assert.IsTrue(level.Contains(cycledCounter));           
        }
    }
}
