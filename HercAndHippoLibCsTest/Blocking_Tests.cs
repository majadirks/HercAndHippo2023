namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Blocking_Tests
    {

        [TestMethod]
        public void PlayerBlockedByBlocker_All4Directions_Test()
        {
            // Arrange
            Player player = Player.Default((3, 3));
            Blocker blocker = new((3, 2)); // initially north of player
            Wall corner = new(ConsoleColor.Black, (20, 20));
            Level level = new(player, new HashSet<HercAndHippoObj> { blocker, corner });
            Assert.IsTrue(blocker.IsBlocking);

            // Assert
            Assert.IsTrue(player.IsBlocked(level, Direction.North));
            Assert.IsFalse(player.IsBlocked(level, Direction.East));
            Assert.IsFalse(player.IsBlocked(level, Direction.South));
            Assert.IsFalse(player.IsBlocked(level, Direction.West));

            // Re-arrange and assert
            Level blockEast = level.Replace(blocker, new Blocker((4, 3))); // blocker east of player
            Assert.IsFalse(player.IsBlocked(blockEast, Direction.North));
            Assert.IsTrue(player.IsBlocked(blockEast, Direction.East));
            Assert.IsFalse(player.IsBlocked(blockEast, Direction.South));
            Assert.IsFalse(player.IsBlocked(blockEast, Direction.West));

            // Re-arrange and assert
            Level blockSouth = level.Replace(blocker, new Blocker((3, 4))); // blocker south of south
            Assert.IsFalse(player.IsBlocked(blockSouth, Direction.North));
            Assert.IsFalse(player.IsBlocked(blockSouth, Direction.East));
            Assert.IsTrue(player.IsBlocked(blockSouth, Direction.South));
            Assert.IsFalse(player.IsBlocked(blockSouth, Direction.West));

            // Re-arrange and assert
            Level blockWest = level.Replace(blocker, new Blocker((2, 3))); // blocker west of south
            Assert.IsFalse(player.IsBlocked(blockWest, Direction.North));
            Assert.IsFalse(player.IsBlocked(blockWest, Direction.East));
            Assert.IsFalse(player.IsBlocked(blockWest, Direction.South));
            Assert.IsTrue(player.IsBlocked(blockWest, Direction.West));
        }

        // ToDo: verify blocked on level boundaries

    }
}
