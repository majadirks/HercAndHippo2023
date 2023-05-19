using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Environment_Tests
    {

        [TestMethod]
        public void WallStopsPlayer_Test()
        {
            // Arrnage
            Player initial = Player.Default((2, 2));
            Player secondPosition = Player.Default((3, 2));
            Player impossiblePosition = Player.Default((4, 2));
            Wall wall = new(ConsoleColor.White, (4, 2));
            Wall boundary = new(ConsoleColor.White, (5, 2));
            Level level = new(initial, new HashSet<IDisplayable> { wall, boundary });

            // Act and Assert
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // Player is adjacent to to wall after moving east
            level = level.RefreshCyclables(ActionInput.MoveEast);
            Assert.IsTrue(level.Contains(secondPosition)); // Wall has blocked further eastward movement.
            Assert.IsFalse(level.Contains(impossiblePosition));
        }

    }
}
