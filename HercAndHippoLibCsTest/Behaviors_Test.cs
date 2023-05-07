using HercAndHippoLibCs;

namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Behaviors_Test
    {
        [TestMethod]
        public void DieAndStopBullet_Test()
        {
            // Arrange
            Bullet bullet = new((2, 1), Direction.East);
            BreakableWall breakableWall = new(ConsoleColor.Green, (3, 1));
            Level initialState = new(
                player: new((1, 1), Health: 100, AmmoCount: 5, Inventory: new()),
                displayables: new()  { bullet, breakableWall });
            Level expectedEndingState = initialState.Without(bullet).Without(breakableWall);
            // Act
            Level actualEndingState = Behaviors.DieAndStopBullet(breakableWall, initialState, bullet);
            // Assert
            Assert.IsTrue(expectedEndingState.HasSameStateAs(actualEndingState));
        }

        [TestMethod]
        public void NoReaction_Test()
        {

            // Arrange
            Level level = new(player: new((2, 3), Health: 100, AmmoCount: 5, Inventory: new()),
                displayables: new()
                {
                    new BreakableWall(ConsoleColor.Green, (1,2)),
                    new BreakableWall(ConsoleColor.Green, (2,2)),
                    new BreakableWall(ConsoleColor.Green, (3,2)),
                    new BreakableWall(ConsoleColor.Green, (1,3)),
                    new BreakableWall(ConsoleColor.Green, (3,3))
                });
            // Act
            Level nextState = Behaviors.NoReaction(level);
            // Assert
            Assert.IsTrue(level.HasSameStateAs(nextState));
        }
    }
}