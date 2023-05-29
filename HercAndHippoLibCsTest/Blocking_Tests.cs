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
            Wall corner = new(Color.Black, (20, 20));
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

        [TestMethod]
        public void BlockedOnBoundaries_Test()
        {
            // Arrange
            Player player = Player.Default((5, 5));
            Noninteractor northNi = new((5, Row.MIN_ROW));
            Noninteractor eastNi = new((10, 5));
            Noninteractor southNi = new((5, 10));
            Noninteractor westNi = new((Column.MIN_COL, 5));
            Level level = new(player, new HashSet<HercAndHippoObj> {northNi, eastNi, southNi, westNi});

            // Assert
            Assert.AreEqual(10, level.Width);
            Assert.AreEqual(10, level.Height);

            Assert.IsTrue(northNi.IsBlocked(level, Direction.North));
            Assert.IsFalse(northNi.IsBlocked(level, Direction.South));

            Assert.IsTrue(eastNi.IsBlocked(level, Direction.East));
            Assert.IsFalse(eastNi.IsBlocked(level, Direction.West));

            Assert.IsTrue(southNi.IsBlocked(level, Direction.South));
            Assert.IsFalse(southNi.IsBlocked(level, Direction.North));

            Assert.IsTrue(westNi.IsBlocked(level, Direction.West));
            Assert.IsFalse(westNi.IsBlocked(level, Direction.East));
        }
    }
}
