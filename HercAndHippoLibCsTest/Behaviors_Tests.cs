
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
        public void DieBehavior_Test()
        {
            // Arrange
            Door door = new(Color.Magenta, (2, 1));
            Key key = new(Color.Magenta, (1, 1));
            Player startPlayer = new((1, 1), health: 100, ammoCount: 5, inventory: new(key));
            Level initialState = new(player: startPlayer, gravity: Gravity.None, secondaryObjects: new() { door });
            Level expectedNextState = initialState.Without(door);
            // Act
            Level actualNextState = door.Die(level: initialState);
            // Assert
            Assert.IsTrue(expectedNextState.HasSameStateAs(actualNextState));
        }
    }
}