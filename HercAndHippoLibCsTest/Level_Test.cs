﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Level_Test
    {
        [TestMethod]
        public void HeightAndWidth_Test()
        {
            // Arrange
            int expectedWidth = 4;
            int expectedHeight = 7;
            Player player = new((1, 1), Health: 100, AmmoCount: 0, Inventory: new HashSet<ITakeable>());
            Level level = new(player: player, displayables: new HashSet<IDisplayable>()
            {
                new Wall(ConsoleColor.Yellow, new Location(Col: expectedWidth, Row: expectedHeight))
            }) ;

            // Assert
            Assert.AreEqual(expectedHeight, level.Height);
            Assert.AreEqual(expectedWidth, level.Width);
        }

        [TestMethod]
        public void HeightAndWidthDoNotChange_Test()
        {
            // Arrange for shooting east
            int expectedWidth = 4;
            int expectedHeight = 7;
            Player player = new(new Location(Col: expectedWidth, Row: expectedHeight), Health: 100, AmmoCount: 5, Inventory: new HashSet<ITakeable>());
            Level level = new(player: player, displayables: new HashSet<IDisplayable>()
            {
                new Wall(ConsoleColor.Yellow, new Location(Col: expectedWidth, Row: expectedHeight))
            });
            Bullet expectedBulletEastOfBoundary = new((expectedWidth + 1, expectedHeight), Direction.East);

            // Act
            level = level.RefreshCyclables(ActionInput.ShootEast); // puts a bullet at column 5

            // Assert: There is a bullet east of the bounds of the level, but width has not changed
            Assert.IsTrue(level.LevelObjects.Contains(expectedBulletEastOfBoundary));
            Assert.IsTrue(expectedBulletEastOfBoundary.Location.Col > expectedWidth);
            Assert.AreEqual(expectedWidth, level.Width);

            // Arrange for shooting south
            Bullet expectedBulletSouthOfBoundary = new(Location: (expectedWidth, expectedHeight + 1), Direction.South);
            // Act
            level = level.RefreshCyclables(ActionInput.ShootSouth); // puts a bullet in row 8
            // Assert: There is a bullet south of the bounds of the level, but height has not changed
            Assert.IsTrue(level.LevelObjects.Contains(expectedBulletSouthOfBoundary));
            Assert.IsTrue(expectedBulletSouthOfBoundary.Location.Row > expectedHeight);
            Assert.AreEqual(expectedHeight, level.Height);
        }
    }
}