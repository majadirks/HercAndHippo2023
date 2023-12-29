/*
 * Tests:
 * Can jump while hippo is locked
 * Pick up hippo on touch if not blocked above
 * Do not pick up hippo if blocked above
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
 */

namespace HercAndHippoLibCsTest;

[TestClass]
public class Hippo_Tests
{
}
