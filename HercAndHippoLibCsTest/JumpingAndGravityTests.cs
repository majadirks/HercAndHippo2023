namespace HercAndHippoLibCsTest;
/*
Player's fall is blocked by a wall
Player's fall is not blocked by ammo
Player collects ammo when falling through ammo
Jumping: Player moves up and then down, with Kinetic Energy changing as intended
When jumping, if player hits an obstacle (wall), Kinetic Energy is set to zero and player falls
 */

[TestClass]
public class JumpingAndGravityTests
{
    [TestMethod]
    public void KineticEnergyCannotBeNegative_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 10)) with { JumpStrength = 5 };
        Level level = new(player: player, gravity: Gravity.Default, secondaryObjects: new());
        Assert.AreEqual(0, (int)level.Player.KineticEnergy);

        // Act: Try to set KineticEnergy to a negative value.
        // That won't work; the kinetic energy will remain at zero
        level = level.WithPlayer(player with { KineticEnergy = - 5 });

        // Assert
        Assert.AreEqual(0, (int)level.Player.KineticEnergy);

        // Act: Try to add five to KineticEnergy. This will work.
        level = level.WithPlayer(player with { KineticEnergy = 5 });

        // Assert
        Assert.AreEqual(5, (int)level.Player.KineticEnergy);
    }
    [TestMethod]
    public void PlayerFallsPerCycleEqualsGraviyStrength_Test()
    {
        for (int gravStrength = 1; gravStrength <= 5; gravStrength++)
        {
            // Arrange
            Player player = Player.Default(new Location(5, 10)) with { JumpStrength = 5 };
            Gravity gravity = new(Strength: gravStrength, WaitCycles: 1);
            Level level = new(
                player: player,
                gravity: gravity,
                secondaryObjects: new()
                {
                new Wall(Color.Magenta, new(5,11)),
                new Wall(Color.Black, new(20, 20))
                });
            // Player jumps
            level = level.RefreshCyclables(ActionInput.MoveNorth);
            while (level.Player.KineticEnergy > 0)
            {
                level = level.RefreshCyclables(ActionInput.MoveNorth);
            }
            Assert.AreEqual(0, (int) level.Player.KineticEnergy);
            Location apex = level.Player.Location;

            // Act: Player falls
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert
            int expectedRow = Math.Min(10, apex.Row + gravity.Strength);
            Assert.AreEqual(new Location(apex.Col, expectedRow), level.Player.Location);
        }
       

    }

    [TestMethod]
    public void WallBlocksFall_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 1));
        Level level = new(
            player: player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Wall(Color.Magenta, new(5,11)),
                new Wall(Color.Black, new(20, 20))
            });
        
        for (int row = 1; row <= 10; row++)
        {
            // Act: Let player fall
            level = level.RefreshCyclables(ActionInput.NoAction);
            // Assert
            Assert.AreEqual(row, (int) level.Player.Location.Row);
        }

        // Act: refresh cyclables again
        level = level.RefreshCyclables(ActionInput.NoAction);
        // Assert: player is still at row 10, above the wall
        Assert.AreEqual(10, (int) level.Player.Location.Row);
    }

    [TestMethod]
    public void NoDoubleJumpsWhenNotBlockedSouth_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 10)) with { JumpStrength = 5 };
        Level level = new(
            player: player,
            gravity: Gravity.Default,
            secondaryObjects: new()
            {
                new Wall(Color.Magenta, new(5,11)),
                new Wall(Color.Black, new(20, 20))
            });
        // Player jumps
        Assert.IsTrue(player.MotionBlockedTo(level, Direction.South));
        Assert.AreEqual(0, (int) player.KineticEnergy);
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        Assert.AreEqual(4, (int) level.Player.KineticEnergy);
        Assert.IsFalse(level.Player.MotionBlockedTo(level, Direction.South));

        // Act: attempt to double-jump
        level = level.RefreshCyclables(ActionInput.MoveNorth);

        // Assert: double-jump failed; kinetic energy continues to decrease
        Assert.AreEqual(3, (int) level.Player.KineticEnergy);
    }

}
