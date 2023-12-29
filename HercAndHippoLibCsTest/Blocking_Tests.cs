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
            Level level = new(player, secondaryObjects: new HashSet<HercAndHippoObj> { blocker, corner }, gravity: Gravity.None);
            Assert.IsTrue(blocker.IsLocatable);

            // Assert
            Assert.IsTrue(player.ObjectLocatedTo(level, Direction.North));
            Assert.IsFalse(player.ObjectLocatedTo(level, Direction.East));
            Assert.IsFalse(player.ObjectLocatedTo(level, Direction.South));
            Assert.IsFalse(player.ObjectLocatedTo(level, Direction.West));

            // Re-arrange and assert
            Level blockEast = level.Replace(blocker, new Blocker((4, 3))); // blocker east of player
            Assert.IsFalse(player.ObjectLocatedTo(blockEast, Direction.North));
            Assert.IsTrue(player.ObjectLocatedTo(blockEast, Direction.East));
            Assert.IsFalse(player.ObjectLocatedTo(blockEast, Direction.South));
            Assert.IsFalse(player.ObjectLocatedTo(blockEast, Direction.West));

            // Re-arrange and assert
            Level blockSouth = level.Replace(blocker, new Blocker((3, 4))); // blocker south of south
            Assert.IsFalse(player.ObjectLocatedTo(blockSouth, Direction.North));
            Assert.IsFalse(player.ObjectLocatedTo(blockSouth, Direction.East));
            Assert.IsTrue(player.ObjectLocatedTo(blockSouth, Direction.South));
            Assert.IsFalse(player.ObjectLocatedTo(blockSouth, Direction.West));

            // Re-arrange and assert
            Level blockWest = level.Replace(blocker, new Blocker((2, 3))); // blocker west of south
            Assert.IsFalse(player.ObjectLocatedTo(blockWest, Direction.North));
            Assert.IsFalse(player.ObjectLocatedTo(blockWest, Direction.East));
            Assert.IsFalse(player.ObjectLocatedTo(blockWest, Direction.South));
            Assert.IsTrue(player.ObjectLocatedTo(blockWest, Direction.West));
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
            Level level = new(player, secondaryObjects: new HashSet<HercAndHippoObj> {northNi, eastNi, southNi, westNi}, gravity: Gravity.None);

            // Assert
            Assert.AreEqual(10, level.Width);
            Assert.AreEqual(10, level.Height);

            Assert.IsTrue(northNi.ObjectLocatedTo(level, Direction.North));
            Assert.IsFalse(northNi.ObjectLocatedTo(level, Direction.South));

            Assert.IsTrue(eastNi.ObjectLocatedTo(level, Direction.East));
            Assert.IsFalse(eastNi.ObjectLocatedTo(level, Direction.West));

            Assert.IsTrue(southNi.ObjectLocatedTo(level, Direction.South));
            Assert.IsFalse(southNi.ObjectLocatedTo(level, Direction.North));

            Assert.IsTrue(westNi.ObjectLocatedTo(level, Direction.West));
            Assert.IsFalse(westNi.ObjectLocatedTo(level, Direction.East));
        }
    }
}
