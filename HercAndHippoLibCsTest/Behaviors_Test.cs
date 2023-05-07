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

            // P*B
            Bullet bullet = new((2, 1), Direction.East);
            BreakableWall breakableWall = new(ConsoleColor.Green, (3, 1));
            Level initialState = new(
                Player: new((1, 1), Health: 100, AmmoCount: 5, Inventory: new()),
                Displayables: new()  { bullet, breakableWall });

            Level expectedEndingState = initialState.Without(bullet).Without(breakableWall);

            // Act
            Level actualEndingState = initialState
                .RefreshCyclables(default) // bullet moves to same location as breakable wall
                .RefreshCyclables(default);

            // Assert
            Assert.IsTrue(expectedEndingState.HasSameStateAs(actualEndingState));

        }
    }
}