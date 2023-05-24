
namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Behaviors_Tests
    {
        [TestMethod]
        public void DieAndStopBullet_Test()
        {
            // Arrange
            Bullet bullet = new((2, 1), Direction.East);
            BreakableWall breakableWall = new(ConsoleColor.Green, (3, 1));
            Level initialState = new(
                player: new((1, 1), Health: 100, AmmoCount: 5, Inventory: new()),
                secondaryObjects: new()  { bullet, breakableWall });
            Level expectedNextState = initialState.Without(bullet).Without(breakableWall);
            // Act
            Level actualNextState = Behaviors.DieAndStopBullet(breakableWall, initialState, bullet);
            // Assert
            Assert.AreEqual(expectedNextState, actualNextState);
        }

        [TestMethod]
        public void NoReaction_Test()
        {
            // Arrange
            Level level = new(player: new((2, 3), Health: 100, AmmoCount: 5, Inventory: new()),
                secondaryObjects: new()
                {
                    new BreakableWall(ConsoleColor.Green, (3,3)),
                });
            // Act
            Level nextState = Behaviors.NoReaction(level);
            // Assert
            Assert.AreEqual(level, nextState);
        }

        [TestMethod]
        public void StopBullet_Test()
        {
            // Arrange
            Bullet bullet = new((3, 2), Direction.South);
            Wall wall = new(ConsoleColor.Green, (3, 3));
            Level level = new(player: new((2, 3), Health: 100, AmmoCount: 5, Inventory: new()),
                secondaryObjects: new() {bullet, wall});
            Level expectedNextState = level.Without(bullet);
            // Act
            Level actualNextState = Behaviors.StopBullet(level, bullet);
            // Assert
            Assert.AreEqual(expectedNextState, actualNextState);
        }

        [TestMethod]
        public void AllowBulletToPass_Test()
        {
            // Arrange
            Bullet bulletAboveKey = new((3, 2), Direction.South);
            Key key = new(ConsoleColor.Green, (3, 3));
            Wall levelCorner = new(ConsoleColor.Blue, (10, 10));
            Level level = new(player: new((5, 5), Health: 100, AmmoCount: 5, Inventory: new()),
                secondaryObjects: new() { bulletAboveKey, key, levelCorner});

            Bullet bulletOverlappingKey = bulletAboveKey with { Location = key.Location };
            Level expectedSecondState = level.Replace(bulletAboveKey, bulletOverlappingKey);

            Bullet bulletPastKey = bulletOverlappingKey with { Location = (3, 4) };
            Level expectedThirdState = expectedSecondState.Replace(bulletOverlappingKey, bulletPastKey);

            // Act
            Level actualSecondState = Behaviors.AllowBulletToPass(key, level, bulletAboveKey);
            Level actualThirdState = actualSecondState.RefreshCyclables(default);
            // Assert
            Assert.AreEqual(expectedSecondState, actualSecondState);
            Assert.AreEqual(expectedThirdState, actualThirdState);
        }

        [TestMethod]
        public void DieAndAllowPassage_Test()
        {
            // Arrange
            Door door = new(ConsoleColor.Magenta, (2, 1));
            Key key = new(ConsoleColor.Magenta, (1, 1));
            Player startPlayer = new((1, 1), Health: 100, AmmoCount: 5, Inventory: new(key));
            Level initialState = new(player: startPlayer, secondaryObjects: new() { door });
            Level expectedNextState = initialState.WithPlayer(startPlayer with { Location = door.Location }).Without(door);
            // Act
            Level actualNextState = Behaviors.DieAndAllowPassage(level: initialState, passedOver: door, passerOver: startPlayer);
            // Assert
            Assert.AreEqual(expectedNextState, actualNextState);
        }
    }
}