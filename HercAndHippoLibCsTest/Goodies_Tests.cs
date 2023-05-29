namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Goodies_Tests
    {
        [TestMethod]
        public void AmmoCanBePickedUp_Test()
        {
            // Arrange
            int ammoCount = 17;
            Ammo ammo = new((2, 1), Count: ammoCount);
            Level level = new(Player.Default(1, 1), new HashSet<HercAndHippoObj>() { ammo });
            Assert.AreEqual(0, (int) level.Player.AmmoCount);
            Assert.IsTrue(level.Contains(ammo));
            Assert.AreNotEqual(ammo.Location, level.Player.Location);
            // Act
            level = level.RefreshCyclables(ActionInput.MoveEast);
            // Assert
            Assert.AreEqual(ammoCount, (int) level.Player.AmmoCount);
            Assert.IsFalse(level.Contains(ammo));
            Assert.AreEqual(ammo.Location, level.Player.Location);
        }

        [TestMethod]
        public void KeyCanBePickedUp_Test()
        {
            // Arrange
            Key key = new(Color.Magenta, (2,1));
            Level level = new(Player.Default(1, 1), new HashSet<HercAndHippoObj>() { key });
            Assert.AreEqual(Inventory.EmptyInventory, level.Player.Inventory);
            Assert.IsTrue(level.Contains(key));
            Assert.AreNotEqual(key.Location, level.Player.Location);
            // Act
            level = level.RefreshCyclables(ActionInput.MoveEast);
            // Assert
            Assert.AreEqual(new Inventory(key), level.Player.Inventory);
            Assert.IsFalse(level.Contains(key));
            Assert.AreEqual(key.Location, level.Player.Location);
        }
    }
}
