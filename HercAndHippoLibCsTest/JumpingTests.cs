﻿namespace HercAndHippoLibCsTest;

[TestClass]
public class JumpingTests
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
    public void AmmoDoesNotBlockFall_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 1));
        Level level = new(
            player: player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Ammo((5,5), 5),
                new Wall(Color.Black, new(5, 11))
            });

        for (int row = 1; row <= 10; row++)
        {
            // Act: Let player fall
            level = level.RefreshCyclables(ActionInput.NoAction);
            // Assert: player has fallen, even through ammo
            Assert.AreEqual(row, (int)level.Player.Location.Row);
        }

        // Act: refresh cyclables again
        level = level.RefreshCyclables(ActionInput.NoAction);
        // Assert: player is still at row 10, above the wall
        Assert.AreEqual(10, (int)level.Player.Location.Row);
        // And ammo is not present
        Assert.IsFalse(level.LevelObjects.Where(obj => obj is Ammo).Any());
    }

    [TestMethod]
    public void TouchObjectsWhileFallingThrough_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 1));
        Level level = new(
            player: player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Ammo((5,5), 5),
                new PassableTouchCounter((5, 6), Count: 0),
                new Wall(Color.Black, new(5, 11))
            });
        Assert.AreEqual(0, (int)player.AmmoCount);
        Assert.AreEqual(0, FindTouchCounter().Count);

        // Act: Let player fall
        for (int row = 1; row <= 10; row++)
        {
            level = level.RefreshCyclables(ActionInput.NoAction);
            Assert.AreEqual(row, (int)level.Player.Location.Row);
        }

        // Assert: player has collected ammo, and touch counter has incremented
        Assert.AreEqual(5, (int)level.Player.AmmoCount);
        Assert.AreEqual(1, FindTouchCounter().Count);

        // Local function
        PassableTouchCounter FindTouchCounter() 
            => (PassableTouchCounter)level.LevelObjects.Where(obj => obj is PassableTouchCounter).Single();
    }

    [TestMethod]
    public void PassThroughDoorsIfPlayerHasKeys_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 1))
        .Take(new Key(Color.Yellow, (0, 0)))
        .Take(new Key(Color.Green, (0, 0)));
        Assert.IsTrue(player.Has<Key>(Color.Yellow));
        Assert.IsTrue(player.Has<Key>(Color.Green));
        Assert.IsFalse(player.Has<Key>(Color.Blue));

        Level level = new(
            player: player,
            gravity: new Gravity(Strength: 1, WaitCycles: 1),
            secondaryObjects: new()
            {
                new Door(Color.Yellow, (5,5)),
                new Door(Color.Green, (5, 6)),
                new Door(Color.Blue, new(5, 11))
            });

        Assert.IsTrue(DoorExists(Color.Yellow));
        Assert.IsTrue(DoorExists(Color.Green));
        Assert.IsTrue(DoorExists(Color.Blue));

        // Act: Let player fall
        for (int row = 1; row <= 10; row++)
        {
            level = level.RefreshCyclables(ActionInput.NoAction);
            Assert.AreEqual(row, (int)level.Player.Location.Row);
        }

        // Let one more cycle pass; player does not pass through the blue door at row 11
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(10, (int)level.Player.Location.Row);
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.South));

        // Assert: player has passed through the yellow and green doors, but not the blue
        Assert.IsFalse(DoorExists(Color.Yellow));
        Assert.IsFalse(DoorExists(Color.Green));
        Assert.IsTrue(DoorExists(Color.Blue));

        // Local function
        bool DoorExists(Color color)
            => level.LevelObjects.Where(obj => obj is Door door &&  door.BackgroundColor == color).Any();
    }

    [TestMethod]
    public void SimpleJump_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 10)) with { JumpStrength = 5 };
        Gravity gravity = new(Strength: 1, WaitCycles: 1);
        Level level = new(
            player: player,
            gravity: gravity,
            secondaryObjects: new()
            {
                new Wall(Color.Magenta, new(5,11)),
                new Wall(Color.Black, new(20, 20))
            });


        // Act: Player jumps
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        Assert.AreEqual(new KineticEnergy(4), level.Player.KineticEnergy);
        Assert.AreEqual(new Location(5,9), level.Player.Location);

        for (int i = 1; i <= 4; i++)
        {
            level = level.RefreshCyclables(ActionInput.NoAction);
            // Assert: Player has moved North, and kinetic energy has decreased
            Assert.AreEqual(new KineticEnergy(4 - i), level.Player.KineticEnergy);
            Assert.AreEqual(new Location(5, 9 - i), level.Player.Location);
        }

        // Kinetic energy is now 0, and player is at apex (5,5).
        Assert.AreEqual(new KineticEnergy(0), level.Player.KineticEnergy);
        Assert.AreEqual(new Location(5, 5), level.Player.Location);

        // Player now falls
        for (int row = 6; row <= 10; row++)
        {
            level = level.RefreshCyclables(ActionInput.NoAction);
            Assert.AreEqual(new KineticEnergy(0), level.Player.KineticEnergy);
            Assert.AreEqual(row, (int) level.Player.Location.Row);
        }

        // Now above wall. Cycle again; player stays put.
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new KineticEnergy(0), level.Player.KineticEnergy);
        Assert.AreEqual(10, (int)level.Player.Location.Row);
        Assert.IsTrue(level.Player.MotionBlockedTo(level, Direction.South));

    }

    [TestMethod]
    public void WallBlocksJump_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 10)) with { JumpStrength = 5 };
        Gravity gravity = new(Strength: 1, WaitCycles: 1);
        Level level = new(
            player: player,
            gravity: gravity,
            secondaryObjects: new()
            {
                new Wall(Color.Magenta, new(5, 6)),
                new Wall(Color.Magenta, new(5,11)),
                new Wall(Color.Black, new(20, 20))
            });


        // Act and assert: Player jumps. Moves North until hitting wall
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        Assert.AreEqual(new KineticEnergy(4), level.Player.KineticEnergy);
        Assert.AreEqual(new Location(5, 9), level.Player.Location);

        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new KineticEnergy(3), level.Player.KineticEnergy);
        Assert.AreEqual(new Location(5, 8), level.Player.Location);

        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new KineticEnergy(2), level.Player.KineticEnergy);
        Assert.AreEqual(new Location(5, 7), level.Player.Location);

        // Player hits wall at this point; kinetic energy falls to zero
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new KineticEnergy(0), level.Player.KineticEnergy);
        Assert.AreEqual(new Location(5, 7), level.Player.Location);

        // Player begins to fall
        level = level.RefreshCyclables(ActionInput.NoAction);
        Assert.AreEqual(new KineticEnergy(0), level.Player.KineticEnergy);
        Assert.AreEqual(new Location(5, 8), level.Player.Location);
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
