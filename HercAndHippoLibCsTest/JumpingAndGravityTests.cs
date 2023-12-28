using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCsTest;
/*
Player can jump when blocked south
When gravity is nonzero, player falls by (gravity) locations per cycle
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
    public void NoDoubleJumpsWhenNotBlockedSouth_Test()
    {
        // Arrange
        Player player = Player.Default(new Location(5, 10)) with { JumpStrength = 5 };
        Level level = new Level(
            player: player,
            gravity: Gravity.Default,
            secondaryObjects: new()
            {
                new Wall(Color.Magenta, new(5,11)),
                new Wall(Color.Black, new(20, 20))
            });
        // Player jumps
        Assert.IsTrue(player.MotionBlockedTo(level, Direction.South));
        Assert.AreEqual(0, player.KineticEnergy);
        level = level.RefreshCyclables(ActionInput.MoveNorth);
        Assert.AreEqual(4, level.Player.KineticEnergy);
        Assert.IsFalse(level.Player.MotionBlockedTo(level, Direction.South));

        // Act: attempt to double-jump
        level = level.RefreshCyclables(ActionInput.MoveNorth);

        // Assert: double-jump failed; kinetic energy continues to decrease
        Assert.AreEqual(3, level.Player.KineticEnergy);

    }
}
