/*
 * Tests:
 * (If player blocked East, but not hippo, can set hippo down atop blockage)
 * (If player blocked West, hippo blocked East, can set hippo down atop west blockage)
 *
 * Hippo prevents east/west motion if it is blocked east/west
 * When hippo is picked up, it is present in inventory and LockedToPlayer is true.
 * If player moves, Location updates for Hippo in LevelObjects
 * When hippo is dropped, it is no longer present in inventory and LockedToPlayer is false
 * Multiple hippos cannot be added to inventory
 * Hippo is subject to gravity (if dropped over hole, falls)
 * Hippo can be placed on top of object (eg if blocked East, but there is space above that blockage, can be placed atop)
 * Player TryGetHippo() returns a hippo if present, null if not, and the returned hippo has LockedToPlayer set to true
 * Test behavior of HippoBlocksTo() method in HippoMotionBlockages record
 * If hippo falls on player, player takes Hippo.
 * Hippo blocks motion when  blocked in opposite direction (whither),
 * but does not block player when player is above

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
    public void PlayerCanPickUpEastHippo_Test()
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
    public void PlayerCanPickUpWestHippo_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 5, Row: 10)) with { JumpStrength = 5 };
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
        level = level.RefreshCyclables(ActionInput.MoveWest);

        // Assert: Hippo is now locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(new Location(4, 9), hippo.Location);
    }

    [TestMethod]
    public void PlayerCanPickUpHippoWhileJumping_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (3,7), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Player jumps, Hippo falls
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        Assert.AreEqual(new Location(3, 9), level.Player.Location);
        level.TryGetHippo(out Hippo? hippo);
        Assert.IsTrue(hippo != null && new Location(3, 7) == hippo.Location);

        // Act: Player rises, hippo falls. Hippo locks to player.
        level = level.RefreshCyclables(ActionInput.NoAction);
        level.TryGetHippo(out hippo);
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo.LockedToPlayer);
        Assert.IsTrue(level.Player.Below(hippo.Location));
        Assert.AreEqual(new Location(3, 8), level.Player.Location);
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

    [TestMethod]
    public void CannotPickUpHippoIfHippoBlockedAbove_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (4,9)),
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);

        // Assert: Hippo is not locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsFalse(hippo == null || hippo.LockedToPlayer);
        Assert.AreEqual(new Location(3, 10), level.Player.Location); // player did not move (blocked by hippo)
        Assert.AreEqual(new Location(4, 10), hippo.Location); // hippo did not move
    }

    [TestMethod]
    public void CannotPickUpHippoIfPlayerBlockedAbove_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (3,9)),
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);

        // Assert: Hippo is not locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsFalse(hippo == null || hippo.LockedToPlayer);
        Assert.AreEqual(new Location(3, 10), level.Player.Location); // player did not move (blocked by hippo)
        Assert.AreEqual(new Location(4, 10), hippo.Location); // hippo did not move
    }

    [TestMethod]
    public void TakeHippoWhenFallingFrom3Above_Test()
    {
        // Arrange
        Level level = new(
            player : Player.Default(new Location(Col: 3, Row: 7)) with { JumpStrength = 5 },
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (3,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        level.ForceSetCycles(1); // Gravity doesn't apply on cycle 0, so force set to 1

        // Act: Player falls
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new Location(3, 8), level.Player.Location);
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new Location(3, 9), level.Player.Location);
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new Location(3, 10), level.Player.Location);

        // Assert: Hippo is now locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(3, 9), hippo.Location);
    }

    [TestMethod]
    public void TakeHippoWhenFallingFrom2Above_Test()
    {
        // Arrange
        Level level = new(
            player: Player.Default(new Location(Col: 3, Row: 8)) with { JumpStrength = 5 },
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (3,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        level.ForceSetCycles(1); // Gravity doesn't apply on cycle 0, so force set to 1

        // Act: Player falls
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new Location(3, 9), level.Player.Location);
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new Location(3, 10), level.Player.Location);

        // Assert: Hippo is now locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(3, 9), hippo.Location);
    }

    [TestMethod]
    public void TakeHippoWhenFallingFromDirectlyAbove_Test()
    {
        // Arrange
        Level level = new(
            player: Player.Default(new Location(Col: 3, Row: 9)) with { JumpStrength = 5 },
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (3,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        level.ForceSetCycles(1); // Gravity doesn't apply on cycle 0, so force set to 1

        // Act: Player falls into hippo
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new Location(3, 10), level.Player.Location);

        // Assert: Hippo is now locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(3, 9), hippo.Location);
    }

    [TestMethod]
    public void PlayerPicksUpHippoByWalkingOver_Test()
    {
        /*
         *  *   ☻   >    ☻    >   H
            *   █H  >   █H    >  █☻
            *  █████   ███      ███
        */
        // Arrange
        Level level = new(
            player: Player.Default(new Location(Col: 3, Row: 9)) with { JumpStrength = 5 },
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, Location: new(3,10)),
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Player walks over hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        Assert.AreEqual(new Location(4, 9), level.Player.Location);
        // and then falls
        level = level.RefreshCyclables(ActionInput.NoAction);
                
        // Assert: Hippo is now locked to player
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 9), hippo.Location);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);

    }

    [TestMethod]
    public void PutDownHippoEast_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10));
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

        // Pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(new Location(4, 9), hippo.Location);

        // Act: Put down hippo
        level = level.RefreshCyclables(ActionInput.DropHippo);

        // Assert
        Assert.IsTrue(level.TryGetHippo(out hippo));
        Assert.IsFalse(hippo == null || hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(new Location(5, 10), hippo.Location);      
    }

    [TestMethod]
    public void PutDownHippoWest_WhenPlayerAndHippoBothBlockedEast_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, Location: (5,9)),
                new Wall(Color.White, Location: (5,10)),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(new Location(4, 9), hippo.Location);

        // Move east so both player and hippo are blocked East by walls
        level = level.RefreshCyclables(ActionInput.MoveEast);
        Assert.IsTrue(level.TryGetHippo(out hippo));
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.East));
        Assert.IsTrue(hippo != null && hippo.MotionBlockedTo(level, Direction.East));
        Assert.AreEqual(new Location(4, 10), level.Player.Location);

        // Act: Put down hippo
        level = level.RefreshCyclables(ActionInput.DropHippo);

        // Assert: Hippo is put down to west
        Assert.IsTrue(level.TryGetHippo(out hippo));
        Assert.IsFalse(hippo == null || hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(new Location(3, 10), hippo.Location);
    }

    [TestMethod]
    public void CannotDropHippoWhenHippoBlockedToBothSides()
    {
        /*
            *  
            *  █H█
            *   ☻
            *  ████
            *  
        */
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (3,9), Health: 10, LockedToPlayer: true),

                new Wall(Color.White, (2,9)),
                new Wall(Color.White, (4,9)),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Try to drop hippo
        level = level.RefreshCyclables(ActionInput.DropHippo);

        // Assert: Hippo still locked to player and positioned above
        level.TryGetHippo(out Hippo? hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.IsTrue(level.Player.Below(hippo.Location));
    }

    [TestMethod]
    public void HippoBlocksWhenHippoBlockedAbove_HippoEastOfPlayer()
    {
        /*
            *      █
            *     ☻H   < player cannot move East
         */

        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10));
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (4,9)),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.East));

        // Act: try to move East
        level = level.RefreshCyclables(ActionInput.MoveEast);

        // Assert: Hippo has blocked motion
        Assert.AreEqual(player.Location, level.Player.Location);
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null);
        Assert.AreEqual(new Location(4,10), hippo.Location);
        Assert.IsFalse(hippo.LockedToPlayer);
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.East));

    }

    [TestMethod]
    public void HippoBlocksWhenHippoBlockedAbove_WestOfPlayer()
    {
        /*
            *      █
            *      H☻   < player cannot move West
         */
        // Arrange
        Player player = Player.Default(new Location(Col: 5, Row: 10));
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (4,10), Health: 10, LockedToPlayer: false),

                new Wall(Color.White, (4,9)),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.West));

        // Act: try to move East
        level = level.RefreshCyclables(ActionInput.MoveWest);

        // Assert: Hippo has blocked motion
        Assert.AreEqual(player.Location, level.Player.Location);
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null);
        Assert.AreEqual(new Location(4, 10), hippo.Location);
        Assert.IsFalse(hippo.LockedToPlayer);
    }

    [TestMethod]
    public void SingleHippoExistsAfterDropping()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10));
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
        Assert.AreEqual(1, level.LevelObjects.Where(obj => obj is Hippo).Count());

        // Pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(1, level.LevelObjects.Where(obj => obj is Hippo).Count());

        // Act: Put down hippo
        level = level.RefreshCyclables(ActionInput.DropHippo);

        // Assert
        Assert.IsTrue(level.TryGetHippo(out hippo));
        Assert.IsFalse(hippo == null || hippo.LockedToPlayer);
        Assert.AreEqual(1, level.LevelObjects.Where(obj => obj is Hippo).Count());
    }

    [TestMethod]
    public void HippoHealthDecreasesWhenShot()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { AmmoCount = 5 };
        Health hippoStartHealth = 10;
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (5,10), Health: hippoStartHealth, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        // Act
        level = level.RefreshCyclables(ActionInput.ShootEast);
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);
        // Assert
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.Health < hippoStartHealth);
        Assert.AreEqual(hippo.Health, hippoStartHealth - Hippo.HEALTH_PENALTY_ON_SHOT);
    }

    [TestMethod]
    public void HippoDiesWhenOutOfHealth()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { AmmoCount = 5 };
        Health hippoStartHealth = Hippo.HEALTH_PENALTY_ON_SHOT;
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Hippo(Location: (5,10), Health: hippoStartHealth, LockedToPlayer: false),

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsNotNull(hippo);
        // Act
        level = level.RefreshCyclables(ActionInput.ShootEast);
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);
        // At this point hippo may still exist but be out of health. Will die in next cycle
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert
        Assert.IsFalse(level.TryGetHippo(out hippo));
        Assert.IsNull(hippo);
    }

    [TestMethod]
    public void WhenPlayerJumps_HippoBlocksAboveWhenHippoBlockedAbove_Test()
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

                new Wall(Color.White, (4, 7))
            });

        level = level.RefreshCyclables(ActionInput.MoveEast);
        Assert.IsTrue(level.TryGetHippo(out Hippo? hippo));
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 9), hippo.Location);
        Assert.IsFalse(hippo.MotionBlockedTo(level, Direction.North));
        Assert.IsFalse(level.Player.MotionBlockedTo(level, Direction.North));

        // Act: Jump
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        level.TryGetHippo(out  hippo);
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(4, 8), hippo.Location);

        // Assert: hippo and player both blocked North
        Assert.IsTrue(hippo.MotionBlockedTo(level, Direction.North));
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.North));

        // Act: Wait a cycle for fall to begin
        level = level.RefreshCyclables(ActionInput.NoAction);
        level.TryGetHippo(out hippo);
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(4, 8), hippo.Location);
        Assert.AreEqual(new Location(4, 9), level.Player.Location);

        level = level.RefreshCyclables(ActionInput.NoAction);
        level.TryGetHippo(out hippo);
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(4, 9), hippo.Location);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(0, (int)level.Player.KineticEnergy);
    }
}
