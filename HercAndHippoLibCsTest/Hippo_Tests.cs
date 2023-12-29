/*
 * Tests:
 * Can jump while hippo is locked
 * Pick up hippo on touch if not blocked above
 * Do not pick up hippo if blocked above
 * Put hippo down East if not blocked East
 * Put hippo down West if blocked East but not West
 * Do not put hippo down if blocked both East and West
 * Hippo health decrements when shot
 * Hippo dies when out of health
 * If player is jumping or moving while hippo is locked on top:
 *   Hippo prevents upward motion if its motion is blocked above
 *   Hippo prevents east/west motion if it is blocked east/west
 * When hippo is picked up, it is present in inventory
 * When hippo is dropped, it is no longer present in inventory
 * Multiple hippos cannot be added to inventory
 */

namespace HercAndHippoLibCsTest;

[TestClass]
public class Hippo_Tests
{
}
