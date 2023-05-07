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
                displayables: new()
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
                displayables: new() {bullet, wall});
            Level expectedNextState = level.Without(bullet);
            // Act
            Level actualNextState = Behaviors.StopBullet(level, bullet);
            // Assert
            Assert.AreEqual(expectedNextState, actualNextState);
        }
    }
}