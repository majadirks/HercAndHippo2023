namespace HercAndHippoLibCsTest;

[TestClass]
public class Direction_Tests
{
    [TestMethod]
    public void SeekDoesNotMoveIntoBlockedPath_Test()
    {
        // Arrange
        Level level = new(
            player: Player.Default(5, 1),
            hippo: null,
            gravity: Gravity.Default,
            secondaryObjects: new()
            {
                    new Wall(Color.Yellow, (5,2)),
                    new Wall(Color.Black, (20,20)),
                    new Wall(Color.Yellow, (9,1)),
                    new Bullet((10, 1), Direction.Seek)
            });
        Bullet initial = FindBullet();
        Assert.AreEqual(new Location(10, 1), initial.Location);
        Assert.AreEqual(Direction.Seek, initial.Whither);

        // Act
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: West and North are blocked, so went South or East
        Bullet iterated = FindBullet();
        Assert.IsTrue(iterated.Location == new Location(10,2) || iterated.Location == new Location(11,1));
        Assert.IsTrue(iterated.Whither == Direction.South || iterated.Whither == Direction.East);

        // Local method
        Bullet FindBullet() => (Bullet)level.LevelObjects.Where(obj => obj is Bullet).Single();
    }

   
}
