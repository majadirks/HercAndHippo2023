using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCsTest;
/*
 * Player cannot double-jump when not blocked south
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
        Player player = Player.Default(new Location(5, 10));
        //Level level = new Level(player, Gravity.Default, 
    }
}
