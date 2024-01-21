namespace HercAndHippoLibCsTest;

[TestClass]
public class DeepSeek_Tests
{
    [TestMethod]
    public void DeepSeek_Depth3_Test()
    {
        // Arrange
        int depth = 3;
        Level level = new(
            player: Player.Default(5, 1),
            hippo: null,
            gravity: Gravity.Default,
            secondaryObjects: new()
            {
                new Wall(Color.Yellow, (5,2)),
                new Wall(Color.Black, (20,20)),
                new Wall(Color.Yellow, (9,1)),
                new Bullet((10, 1), Direction.Idle)
            });
        Bullet initial = FindBullet();
        Assert.AreEqual(new Location(10, 1), initial.Location);
        Assert.AreEqual(Direction.Idle, initial.Whither);

        // Act
        DeepSeekResults results = initial.DeepSeek(level, depth, cameFrom: initial.Location);
        level = level.ReplaceIfPresent(initial, initial with { Location = results.Location });

        // Assert: West and North are blocked, so went South
        Bullet iterated = FindBullet();
        Assert.AreEqual(new Location(10, 2), iterated.Location);
        Assert.AreEqual(4, results.Metric);

        // Act
        results = iterated.DeepSeek(level, depth, cameFrom: iterated.Location);
        level = level.ReplaceIfPresent(iterated,
            iterated with { Location = results.Location });
        iterated = FindBullet();

        // Assert: North is blocked, so went West
        Assert.AreEqual(new Location(9, 2), iterated.Location);
        Assert.AreEqual(3, results.Metric);

        // Act
        results = iterated.DeepSeek(level, depth, cameFrom: iterated.Location);
        level = level.ReplaceIfPresent(iterated,
            iterated with { Location = results.Location });
        iterated = FindBullet();

        // Assert: North is blocked, so went West
        Assert.AreEqual(new Location(8, 2), iterated.Location);
        Assert.AreEqual(2, results.Metric);

        // Act
        results = iterated.DeepSeek(level, depth, cameFrom: iterated.Location);
        level = level.ReplaceIfPresent(iterated,
            iterated with { Location = results.Location });
        iterated = FindBullet();

        results = iterated.DeepSeek(level, depth, cameFrom: iterated.Location);
        level = level.ReplaceIfPresent(iterated,
            iterated with { Location = results.Location });
        iterated = FindBullet();

        results = iterated.DeepSeek(level, depth, cameFrom: iterated.Location);
        level = level.ReplaceIfPresent(iterated,
            iterated with { Location = results.Location });
        iterated = FindBullet();

        results = iterated.DeepSeek(level, depth, cameFrom: iterated.Location);
        level = level.ReplaceIfPresent(iterated,
            iterated with { Location = results.Location });
        iterated = FindBullet();

        Assert.AreEqual(level.Player.Location, iterated.Location);

        // Local method
        Bullet FindBullet() => (Bullet)level.LevelObjects.Where(obj => obj is Bullet).Single();
    }
}
