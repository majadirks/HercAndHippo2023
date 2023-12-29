
namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Behaviors_Tests
    {
        [TestMethod]
        public void NoReaction_Test()
        {
            // Arrange
            Level level = new(player: new((2, 3), health: 100, ammoCount: 5, inventory: new()),
                gravity: Gravity.None,
                secondaryObjects: new()
                {
                    new BreakableWall(Color.Green, (3,3)),
                });
            // Act
            Level nextState = Behaviors.NoReaction(level);
            // Assert
            Assert.AreEqual(level, nextState);
        }

        [TestMethod]
        public void AllowBulletToPass_Test()
        {
            // Arrange
            Bullet bulletAboveKey = new((3, 2), Direction.South);
            Key key = new(Color.Green, (3, 3));
            Wall levelCorner = new(Color.Blue, (10, 10));
            Level level = new(player: new((5, 5), health: 100, ammoCount: 5, inventory: new()),
                gravity: Gravity.None,
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
        public void DieBehavior_Test()
        {
            // Arrange
            Door door = new(Color.Magenta, (2, 1));
            Key key = new(Color.Magenta, (1, 1));
            Player startPlayer = new((1, 1), health: 100, ammoCount: 5, inventory: new(key));
            Level initialState = new(player: startPlayer, gravity: Gravity.None, secondaryObjects: new() { door });
            Level expectedNextState = initialState.Without(door);
            // Act
            Level actualNextState = Behaviors.Die(level: initialState, toDie: door);
            // Assert
            Assert.AreEqual(expectedNextState, actualNextState);
        }
    }
}