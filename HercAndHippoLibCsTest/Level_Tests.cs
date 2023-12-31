using static HercAndHippoLibCs.Inventory;

namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Level_Tests
    {
        [TestMethod]
        public void HeightAndWidth_Test()
        {
            // Arrange
            int expectedWidth = 4;
            int expectedHeight = 7;
            Player player = new((1, 1), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Level level = new(player: player, hippo: null, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>()
            {
                new Wall(Color.Yellow, new Location(Col: expectedWidth, Row: expectedHeight))
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
            Player player = new(new Location(Col: expectedWidth, Row: expectedHeight), health: 100, ammoCount: 5, inventory: EmptyInventory);
            Level level = new(player: player, hippo: null, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>()
            {
                new Wall(Color.Yellow, new Location(Col: expectedWidth, Row: expectedHeight))
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
            Player player = new(new Location(Col: 1, Row: 1), health: 100, ammoCount: 5, inventory: EmptyInventory);
            int startCount = 0;
            CycleCounter initialCounter = new((2, 2), startCount);
            CycleCounter cycledCounter = new((2, 2), startCount + 1);
            Level level = new(player: player, hippo: null, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { initialCounter });

            // Check that we set this up correctly
            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));

            // Act
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert
            Assert.IsFalse(level.Contains(initialCounter));
            Assert.IsTrue(level.Contains(cycledCounter));           
        }

        [TestMethod]
        public void CountCycles_Test()
        {
            // Arrange
            Player player = Player.Default((1, 1));
            Level level = new(player, hippo: null, gravity: Gravity.None, secondaryObjects: new());

            for (int i = 0; i < 100; i++)
            {
                // Assert
                Assert.AreEqual(i, level.Cycles);
                // Act
                level = level.RefreshCyclables(ActionInput.NoAction);
            }
        }

        [TestMethod]
        public void TryGetHippo_ReturnsHippoIfPresent_Test()
        {
            // Arrange
            Level level = new(Player.Default(1, 1), hippo: null, gravity: Gravity.Default, secondaryObjects: new() { new Hippo((2, 2), 5, false) });
            // Act
            Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
            // Assert
            Assert.IsNotNull(hippo);
        }

        [TestMethod]
        public void TryGetHippo_ReturnsNullIfHippoNotPresent_Test()
        {
            // Arrange
            Level level = new(
                player: Player.Default(1, 1), 
                hippo: null, 
                gravity: Gravity.Default, 
                secondaryObjects: new() { new Wall(Color.White, (2, 2)) });
            // Act
            Assert.IsFalse(level.TryGetHippo(out Hippo? hippo));
            // Assert
            Assert.IsNull(hippo);
        }

    }
}
