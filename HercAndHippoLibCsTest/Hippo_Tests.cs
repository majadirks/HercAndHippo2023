/*
 * Tests:
 * Do not pick up hippo if player is blocked above
 * Do not pick up hippo if hippo is blocked above
 * Put hippo down East if not blocked East
 * Put hippo down West if blocked East but not West
 * Do not put hippo down if blocked both East and West
 * After hippo is put down, only one hippo exists on the level.
 *      (Currently there's a bug where, if player is atop a block with space on both sides, hippo is placed both east and west!)
 * Hippo health decrements when shot
 * Hippo dies when out of health
 * If player is jumping or moving while hippo is locked on top:
 *   Hippo prevents upward motion if its motion is blocked above
 *   Hippo prevents east/west motion if it is blocked east/west
 * When hippo is picked up, it is present in inventory and LockedToPlayer is true.
 * If player moves, Location updates for Hippo in inventory
 * If player moves, Location updates for Hippo in LevelObjects
 * When hippo is dropped, it is no longer present in inventory and LockedToPlayer is false
 * Multiple hippos cannot be added to inventory
 * Hippo is subject to gravity (if dropped over hole, falls)
 * Hippo can be placed on top of object (eg if blocked East, but there is space above that blockage, can be placed atop)
 * Player TryGetHippo() returns a hippo if present, null if not, and the returned hippo has LockedToPlayer set to true
 * Test behavior of HippoBlocksTo() method in HippoMotionBlockages record
 * If hippo falls on player, player takes Hippo.
 * If player jumps into falling hippo, player takes Hippo.
 * 
 */

namespace HercAndHippoLibCsTest;

[TestClass]
public class Hippo_Tests
{

    [TestMethod]
    public void PlayerCanJumpWithHippo_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        level = level.RefreshCyclables(ActionInput.MoveEast);
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);

        // Act: Jump
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: Player has jumped, and hippo has jumped with player
        Assert.AreEqual(new Location(4,7), level.Player.Location);
        Assert.IsTrue(level.TryGetHippo(out hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 6), hippo.Location);
    }

    [TestMethod]
    public void PlayerCanPickUpHippo_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);

        // Assert: Hippo is now locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4,10), level.Player.Location);
        Assert.AreEqual(new Location(4, 9), hippo.Location);
    }

    [TestMethod]
    public void DoNotPickUpHippoIfPlayerBlockedAbove_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.Cyan, Location: new(3,9)),
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Assert.IsTrue(player.MotionBlockedTo(level, Direction.North));

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: Hippo is now locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsNotNull(hippo != null);
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        Assert.IsFalse(hippo.LockedToPlayer);
        #pragma warning restore CS8602 // Dereference of a possibly null reference.
        Assert.AreEqual(new Location(4, 10), hippo.Location);
    }

    [TestMethod]
    public void DoNotPickUpHippoIfHippoBlockedAbove_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.Cyan, Location: new(4,9)),
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        level.TryGetHippo(out Hippo? hippo);
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo.MotionBlockedTo(level, Direction.North));

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: Hippo is now locked to player
        Assert.IsTrue(level.TryGetHippo(out hippo));
        Assert.IsNotNull(hippo != null);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Assert.IsFalse(hippo.LockedToPlayer);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        Assert.AreEqual(new Location(4, 10), hippo.Location);
    }
}
