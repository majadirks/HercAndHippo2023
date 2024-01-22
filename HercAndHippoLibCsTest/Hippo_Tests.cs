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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        level = level.RefreshCyclables(ActionInput.MoveEast);
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);

        // Act: Jump
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: Player has jumped, and hippo has jumped with player
        Assert.AreEqual(new Location(4,7), level.Player.Location);
        hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 6), hippo.Location);

        // Act: Continue the jump 
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: Hippo still above player
        Assert.AreEqual(new Location(4, 5), level.Player.Location);

        hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 4), hippo.Location);
        Assert.AreEqual(new KineticEnergy(0), level.Player.KineticEnergy); // reached apex of jump

        // Now fall begins
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: Hippo still above player during fall
        Assert.AreEqual(new Location(4, 6), level.Player.Location);
        hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 5), hippo.Location);
    }

    [TestMethod]
    public void PlayerCanPickUpEastHippo_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);

        // Assert: Hippo is now locked to player
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveWest);

        // Assert: Hippo is now locked to player
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (3, 7), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Player jumps, Hippo falls
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        Assert.AreEqual(new Location(3, 9), level.Player.Location);
        Hippo? hippo = level.Hippo;
        Assert.IsTrue(hippo != null && new Location(3, 7) == hippo.Location);

        // Act: Player rises, hippo falls. Hippo locks to player.
        level = level.RefreshCyclables(ActionInput.NoAction);
        hippo = level.Hippo;
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.Cyan, Location: new(3,9)),
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
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.Cyan, Location: new(4,9)),
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        Hippo? hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo.MotionBlockedTo(level, Direction.North));

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: Hippo is now locked to player
        hippo = level.Hippo; Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (4,9)),              
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);

        // Assert: Hippo is not locked to player
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (3,9)),
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Act: Move to pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);

        // Assert: Hippo is not locked to player
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (3, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
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
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(3, 9), hippo.Location);
    }

    [TestMethod]
    public void TakeHippoWhenFallingFrom2Above_Test()
    {
        // Arrange
        Level level = new(
            player: Player.Default(new Location(Col: 3, Row: 8)) with { JumpStrength = 5 },
            hippo: new Hippo(Location: (3, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
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
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(3, 9), hippo.Location);
    }

    [TestMethod]
    public void TakeHippoWhenFallingFromDirectlyAbove_Test()
    {
        // Arrange
        Level level = new(
            player: Player.Default(new Location(Col: 3, Row: 9)) with { JumpStrength = 5 },
            hippo: new Hippo(Location: (3, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
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
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, Location: new(3,10)),
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
        Hippo? hippo = level.Hippo; 
        Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });

        // Pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(new Location(4, 9), hippo.Location);

        // Act: Put down hippo
        level = level.RefreshCyclables(ActionInput.DropHippo);

        // Assert
        hippo = level.Hippo; Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
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
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(new Location(4, 9), hippo.Location);

        // Move east so both player and hippo are blocked East by walls
        level = level.RefreshCyclables(ActionInput.MoveEast);
        hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.East));
        Assert.IsTrue(hippo != null && hippo.MotionBlockedTo(level, Direction.East));
        Assert.AreEqual(new Location(4, 10), level.Player.Location);

        // Act: Put down hippo
        level = level.RefreshCyclables(ActionInput.DropHippo);

        // Assert: Hippo is put down to west
        hippo = level.Hippo; Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (3, 9), Health: 10, LockedToPlayer: true),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
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
        Hippo? hippo = level.Hippo;
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
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
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            secondaryObjects: new()
            {
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
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Assert.AreEqual(1, level.LevelObjects.Where(obj => obj is Hippo).Count());

        // Pick up hippo
        level = level.RefreshCyclables(ActionInput.MoveEast);
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(1, level.LevelObjects.Where(obj => obj is Hippo).Count());

        // Act: Put down hippo
        level = level.RefreshCyclables(ActionInput.DropHippo);

        // Assert
        hippo = level.Hippo; Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (5, 10), Health: hippoStartHealth, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
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
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
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
            hippo: new Hippo(Location: (5, 10), Health: hippoStartHealth, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsNotNull(hippo);
        // Act
        level = level.RefreshCyclables(ActionInput.ShootEast);
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);
        // At this point hippo may still exist but be out of health. Will die in next cycle
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert
        Assert.IsNull(level.Hippo);
    }

    [TestMethod]
    public void WhenPlayerJumps_HippoBlocksAboveWhenHippoBlockedAbove_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {               
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),

                new Wall(Color.White, (4, 7))
            });

        level = level.RefreshCyclables(ActionInput.MoveEast);
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo != null && hippo.LockedToPlayer);
        Assert.AreEqual(new Location(4, 9), hippo.Location);
        Assert.IsFalse(hippo.MotionBlockedTo(level, Direction.North));
        Assert.IsFalse(level.Player.MotionBlockedTo(level, Direction.North));

        // Act: Jump
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(4, 8), hippo.Location);

        // Assert: hippo and player both blocked North
        Assert.IsTrue(hippo.MotionBlockedTo(level, Direction.North));
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.North));

        // Act: Wait a cycle for fall to begin
        level = level.RefreshCyclables(ActionInput.NoAction);
        hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(4, 8), hippo.Location);
        Assert.AreEqual(new Location(4, 9), level.Player.Location);

        level = level.RefreshCyclables(ActionInput.NoAction);
        hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(4, 9), hippo.Location);
        Assert.AreEqual(new Location(4, 10), level.Player.Location);
        Assert.AreEqual(0, (int)level.Player.KineticEnergy);
    }

    [TestMethod]
    public void HippoBlocksMotionEast_WhenHippoBlockedEastAndPlayerBlockedAbove_HippoEastOfPlayer_Test()
    {
        /*
        *  
        *   █  
        *   ☻H█  << Player blocked East
        *  ████
        *  
        */
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10));
        Level level = new(
            player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            hippo: new Hippo(Location: (4, 10), Health: 10, LockedToPlayer: false),
            secondaryObjects: new()
            {
                new Wall(Color.White, (3,9)), // above player
                new Wall(Color.White, (5,10)), // east of hippo                

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Hippo? hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo.MotionBlockedTo(level, Direction.East));
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.East));

        // Act: Move east
        level = level.RefreshCyclables(ActionInput.MoveEast);

        // Assert: Hippo was picked up
        hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(3, 10), level.Player.Location);
        Assert.AreEqual(new Location(4, 10), hippo.Location);
        Assert.IsFalse(hippo.LockedToPlayer);
    }

    [TestMethod]
    public void HippoBlocksMotionWest_WhenHippoBlockedWestAndPlayerBlockedAbove_HippoWestOfPlayer_Test()
    {
        /*
        *  
        *    █ 
        *  █H☻ 
        *  ████
        *  
        */
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10));
        Level level = new(
            player,
            hippo: new Hippo(Location: (2, 10), Health: 10, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (3,9)), // above player
                new Wall(Color.White, (1,10)), // west of hippo                

                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Hippo? hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo.MotionBlockedTo(level, Direction.West));
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.West));

        // Act: Move west
        level = level.RefreshCyclables(ActionInput.MoveWest);

        // Assert: Hippo was picked up
        hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(3, 10), level.Player.Location);
        Assert.AreEqual(new Location(2, 10), hippo.Location);
        Assert.IsFalse(hippo.LockedToPlayer);
    }

    [TestMethod]
    public void HippoMovesEastWestWithPlayer_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(Col: 3, Row: 10)) with { JumpStrength = 5 };
        Level level = new(
            player,
            hippo: new Hippo(Location: (3, 9), Health: 10, LockedToPlayer: true),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1,11)),
                new Wall(Color.White, (2,11)),
                new Wall(Color.White, (3,11)),
                new Wall(Color.White, (4,11)),
                new Wall(Color.White, (5,11)),
            });
        Hippo? hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo.LockedToPlayer);

        for (int col = 4; col <= 5; col++)
        {
            // Act
            level = level.RefreshCyclables(ActionInput.MoveEast);
            // Assert
            Assert.AreEqual((int) level.Player.Location.Col, col);
            hippo = level.Hippo;
            Assert.IsNotNull(hippo);
            Assert.AreEqual((int)hippo.Location.Col, col);
        }

        for (int col = 4; col >= 1; col--)
        {
            // Act
            level = level.RefreshCyclables(ActionInput.MoveWest);
            // Assert
            Assert.AreEqual((int)level.Player.Location.Col, col);
            hippo = level.Hippo;
            Assert.IsNotNull(hippo);
            Assert.AreEqual((int)hippo.Location.Col, col);
        }
    }

    [TestMethod]
    public void HippoFallsDueToGravity_Test()
    {
        /*
      *  
      *  H      >      
      *  ☻      >   ☻   
      *  █      >   █H   
      * █████   >  █████
      *  
      */

        // Arrange
        Player player = Player.Default(new(2, 2));
        Level level = new(
            player: player,
            hippo: new Hippo((2, 1), Health: 5, LockedToPlayer: true),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (2,3)),

                new Wall(Color.White, (1,4)),
                new Wall(Color.White, (2,4)),
                new Wall(Color.White, (3,4)),
                new Wall(Color.White, (4,4)),
            } );

        // Act: Drop hippo
        level = level.RefreshCyclables(ActionInput.DropHippo);
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert: Hippo has fallen
        Hippo? hippo = level.Hippo;
        Assert.IsNotNull(hippo);
        Assert.AreEqual(new Location(3, 3), hippo.Location);
    }

    [TestMethod]
    public void PlayerTakesHippoIfHippoFallsOnPlayer_Test()
    {
        // Arrange
        Level level = new(
            player: Player.Default((1, 4)),
            hippo: new Hippo((1, 1), Health: 5, LockedToPlayer: false),
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.White, (1, 5)),  
            });

        // Act: Let the hippo fall into the player
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);
        level = level.RefreshCyclables(ActionInput.NoAction);

        // Assert
        Hippo? hippo = level.Hippo; Assert.IsNotNull(hippo);
        Assert.IsNotNull(hippo);
        Assert.IsTrue(hippo.LockedToPlayer);
    }

    [TestMethod]
    public void GetBlockages_HippoPresent_LockedToPlayer_Test()
    {
        // Arrange
        Hippo hippo = new((2, 2), Health: 5, LockedToPlayer: true);
        Level level = new(
            player: Player.Default(2,3), 
            hippo: hippo,
            gravity: Gravity.Default, 
            secondaryObjects: new()
        {
            new BreakableWall(Color.Yellow, (2,1)),
            new Wall(Color.Black, (5,5))
        });

        // Act and Assert
        var blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: true, BlockedEast: false, BlockedWest: false), blockages);

        level = level.ReplaceIfPresent(FindBreakableWall(), new BreakableWall(Color.Yellow, (3, 2)));
        blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: false, BlockedEast: true, BlockedWest: false), blockages);

        level = level.ReplaceIfPresent(FindBreakableWall(), new BreakableWall(Color.Yellow, (1, 2)));
        blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: false, BlockedEast: false, BlockedWest: true), blockages);

        // Local method
        BreakableWall FindBreakableWall () => (BreakableWall)level.LevelObjects.Where(obj => obj is BreakableWall).Single();
    }

    [TestMethod]
    public void GetBlockages_HippoPresent_NotLockedToPlayer_Test()
    {
        // Arrange
        Hippo hippo = new((2, 2), Health: 5, LockedToPlayer: false);
        Level level = new(
            player: Player.Default(1, 1), 
            hippo: hippo,
            gravity: Gravity.Default, 
            secondaryObjects: new()
        {
            new BreakableWall(Color.Yellow, (2,1)),
            new Wall(Color.Black, (5,5))
        });

        // Act and Assert
        var blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: false, BlockedEast: false, BlockedWest: false), blockages);

        level = level.ReplaceIfPresent(FindBreakableWall(), new BreakableWall(Color.Yellow, (3, 2)));
        blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: false, BlockedEast: false, BlockedWest: false), blockages);

        level = level.ReplaceIfPresent(FindBreakableWall(), new BreakableWall(Color.Yellow, (1, 2)));
        blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: false, BlockedEast: false, BlockedWest: false), blockages);

        // Local method
        BreakableWall FindBreakableWall() => (BreakableWall)level.LevelObjects.Where(obj => obj is BreakableWall).Single();
    }

    [TestMethod]
    public void GetBlockages_HippoAbsent_Test()
    {
        // Arrange
        Level level = new(
            player: Player.Default(1, 1), 
            hippo: null,
            gravity: Gravity.Default, 
            secondaryObjects: new()
        {
            new BreakableWall(Color.Yellow, (2,1)),
            new Wall(Color.Black, (5,5))
        });

        // Act and Assert
        Hippo? hippo = level.Hippo;
        Assert.IsNull(hippo);
        var blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: false, BlockedEast: false, BlockedWest: false), blockages);

        level = level.ReplaceIfPresent(FindBreakableWall(), new BreakableWall(Color.Yellow, (3, 2)));
        hippo = level.Hippo;
        Assert.IsNull(hippo);
        blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: false, BlockedEast: false, BlockedWest: false), blockages);

        level = level.ReplaceIfPresent(FindBreakableWall(), new BreakableWall(Color.Yellow, (1, 2)));
        hippo = level.Hippo;
        Assert.IsNull(hippo);
        blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.AreEqual(new HippoMotionBlockages(BlockedNorth: false, BlockedEast: false, BlockedWest: false), blockages);

        // Local method
        BreakableWall FindBreakableWall() => (BreakableWall)level.LevelObjects.Where(obj => obj is BreakableWall).Single();
    }

    [TestMethod]
    public void GetBlockagesByDirection_HippoPresent_LockedToPlayer_Test()
    {
        // Arrange
        Hippo hippo = new((2, 2), Health: 5, LockedToPlayer: true);
        Level level = new(
            player: Player.Default(2, 3), 
            hippo: hippo,
            gravity: Gravity.Default, 
            secondaryObjects: new()
        {
            new BreakableWall(Color.Yellow, (2,1)),
            new Wall(Color.Black, (5,5))
        });

        // Act and Assert
        var blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.IsTrue(blockages.HippoBlocksTo(Direction.North));

        level = level.ReplaceIfPresent(FindBreakableWall(), new BreakableWall(Color.Yellow, (3, 2)));
        blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.IsTrue(blockages.HippoBlocksTo(Direction.East));

        level = level.ReplaceIfPresent(FindBreakableWall(), new BreakableWall(Color.Yellow, (1, 2)));
        blockages = HippoMotionBlockages.GetBlockages(level);
        Assert.IsTrue(blockages.HippoBlocksTo(Direction.West));

        // Local method
        BreakableWall FindBreakableWall() => (BreakableWall)level.LevelObjects.Where(obj => obj is BreakableWall).Single();
    }

}
