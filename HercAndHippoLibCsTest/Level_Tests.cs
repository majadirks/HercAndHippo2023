using static HercAndHippoLibCs.Inventory;

namespace HercAndHippoLibCsTest
{
    [TestClass]
    public class Level_Tests
    {
        [TestMethod]
        public void HeightAndWidth_Test()
        {
            // Arrange
            int expectedWidth = 4;
            int expectedHeight = 7;
            Player player = new((1, 1), health: 100, ammoCount: 0, inventory: EmptyInventory);
            Level level = new(player: player, hippo: null, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>()
            {
                new Wall(Color.Yellow, new Location(Col: expectedWidth, Row: expectedHeight))
            }) ;

            // Assert
            Assert.AreEqual(expectedHeight, level.Height);
            Assert.AreEqual(expectedWidth, level.Width);
        }

        [TestMethod]
        public void HeightAndWidthDoNotChange_Test()
        {
            // Arrange
            int expectedWidth = 4;
            int expectedHeight = 7;
            Player player = new(new Location(Col: expectedWidth, Row: expectedHeight), health: 100, ammoCount: 5, inventory: EmptyInventory);
            Level level = new(player: player, hippo: null, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>()
            {
                new Wall(Color.Yellow, new Location(Col: expectedWidth, Row: expectedHeight))
            });
            Bullet bulletEastOfBoundary = new((expectedWidth + 10, expectedHeight), Direction.East);
            Bullet bulletSouthOfBoundary = new(Location: (expectedWidth, expectedHeight + 1), Direction.South);

            // Act
            level = level.AddSecondaryObject(bulletEastOfBoundary).AddSecondaryObject(bulletSouthOfBoundary);

            // Assert: There is a bullet east of the bounds of the level, but width has not changed.
            Assert.IsTrue(level.LevelObjects.Contains(bulletEastOfBoundary));
            Assert.IsTrue(bulletEastOfBoundary.Location.Col > expectedWidth);
            Assert.IsTrue(bulletEastOfBoundary.Location.Col > level.Width);
            Assert.AreEqual(expectedWidth, level.Width);

            // Assert: There is a bullet south of the bounds of the level, but height has not changed
            Assert.IsTrue(level.LevelObjects.Contains(bulletSouthOfBoundary));
            Assert.IsTrue(bulletSouthOfBoundary.Location.Row > expectedHeight);
            Assert.IsTrue(bulletSouthOfBoundary.Location.Row > level.Height);
            Assert.AreEqual(expectedHeight, level.Height);
        }


        [TestMethod]
        public void RefreshCyclables_Test()
        {
            // Arrange
            Player player = new(new Location(Col: 1, Row: 1), health: 100, ammoCount: 5, inventory: EmptyInventory);
            int startCount = 0;
            CycleCounter initialCounter = new((2, 2), startCount);
            CycleCounter cycledCounter = new((2, 2), startCount + 1);
            Level level = new(player: player, hippo: null, gravity: Gravity.None, secondaryObjects: new HashSet<HercAndHippoObj>() { initialCounter });

            // Check that we set this up correctly
            Assert.IsTrue(level.Contains(initialCounter));
            Assert.IsFalse(level.Contains(cycledCounter));

            // Act
            level = level.RefreshCyclables(ActionInput.NoAction);

            // Assert
            Assert.IsFalse(level.Contains(initialCounter));
            Assert.IsTrue(level.Contains(cycledCounter));           
        }

        [TestMethod]
        public void CountCycles_Test()
        {
            // Arrange
            Player player = Player.Default((1, 1));
            Level level = new(player, hippo: null, gravity: Gravity.None, secondaryObjects: new() { new Wall(Color.Yellow, (2, 2))});

            for (int i = 0; i < 100; i++)
            {
                // Assert
                Assert.AreEqual(i, level.Cycles);
                // Act
                level = level.RefreshCyclables(ActionInput.NoAction);
            }
        }

        [TestMethod]
        public void AttemptToRemoveNullThrowsException()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Level level = new(
                player: player,
                hippo: null,
                gravity: Gravity.None,
                secondaryObjects: new() { new Wall(Color.Yellow, (2, 2)) });
            // Act and Assert.
            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsException<ArgumentNullException>(() => level.Without(null));
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            #pragma warning disable CS8604 // Possible null reference argument.
            Assert.ThrowsException<ArgumentNullException>(() => level.Without(level.Hippo));
            #pragma warning restore CS8604 // Possible null reference argument.
        }

        [TestMethod]
        public void AttemptToRemovePlayerThrowsException()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Level level = new(
                player: player,
                hippo: null,
                gravity: Gravity.None,
                secondaryObjects: new() { new Wall(Color.Yellow, (2, 2)) });
            // Act and Assert.
            Assert.ThrowsException<NotSupportedException>(() => level.Without(player));
        }

        [TestMethod]
        public void RemoveHippo_Test()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Hippo hippo = new((2, 2), 5, false);
            Level level = new(
                player: player,
                hippo: hippo,
                gravity: Gravity.Default,
                secondaryObjects: new() { new Wall(Color.Yellow, (2, 3))});

            Assert.IsNotNull(level.Hippo);
            Assert.IsTrue(level.LevelObjects.Where(obj => obj is Hippo).Any());
            Assert.IsTrue(level.Contains(hippo));

            // Act
            level = level.Without(hippo);
            // Assert
            Assert.IsNull(level.Hippo);
            Assert.IsFalse(level.Contains(hippo));
            Assert.IsFalse(level.LevelObjects.Where(obj => obj is Hippo).Any()); 
        }

        [TestMethod]
        public void RemoveSecondaryObject_Test()
        {
            // Arrange
            Ammo ammo = new((2, 1), 5);
            Level level = new(
                player: Player.Default(1, 1),
                hippo: new((2, 2), 5, false),
                gravity: Gravity.Default,
                secondaryObjects: new() 
                { 
                    new Wall(Color.Yellow, (2, 3)),
                    ammo
                });
            Assert.IsTrue(level.Contains(ammo));

            // Act
            level = level.Without(ammo);
            // Assert
            Assert.IsFalse(level.Contains(ammo));
        }

        [TestMethod]
        public void ReplaceThrowsExceptionIfFirstArgIsNull()
        {
            // Arrange
            Ammo ammo = new((2, 1), 5);
            Level level = new(
                player: Player.Default(1, 1),
                hippo: new((2, 2), 5, false),
                gravity: Gravity.Default,
                secondaryObjects: new()
                {
                    new Wall(Color.Yellow, (2, 3)),
                    ammo
                });

            // Act and assert
            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsException<ArgumentNullException>(() => level.Replace(ammo, null));
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestMethod]
        public void ReplaceThrowsExceptionIfSecondArgIsNull()
        {
            // Arrange
            Ammo ammo = new((2, 1), 5);
            Level level = new(
                player: Player.Default(1, 1),
                hippo: new((2, 2), 5, false),
                gravity: Gravity.Default,
                secondaryObjects: new()
                {
                    new Wall(Color.Yellow, (2, 3)),
                    ammo
                });

            // Act and assert
            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsException<ArgumentNullException>(() => level.Replace(null, ammo));
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestMethod]
        public void CannotReplacePlayerWithNonPlayer()
        {
            // Arrange
            Ammo ammo = new((2, 1), 5);
            Level level = new(
                player: Player.Default(1, 1),
                hippo: new((2, 2), 5, false),
                gravity: Gravity.Default,
                secondaryObjects: new()
                {
                    new Wall(Color.Yellow, (2, 3)),
                    ammo
                });

            // Act and assert
            Assert.ThrowsException<NotSupportedException>(() => level.Replace(level.Player, ammo));
        }

        [TestMethod]
        public void CannotReplaceNonPlayerWithPlayer()
        {
            // Arrange
            Ammo ammo = new((2, 1), 5);
            Level level = new(
                player: Player.Default(1, 1),
                hippo: new((2, 2), 5, false),
                gravity: Gravity.Default,
                secondaryObjects: new()
                {
                    new Wall(Color.Yellow, (2, 3)),
                    ammo
                });

            // Act and assert
            Assert.ThrowsException<NotSupportedException>(() => level.Replace(ammo, level.Player));
        }

        [TestMethod]
        public void CannotReplaceHippoWithNonHippo()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Hippo hippo = new((2, 2), 5, false);
            Ammo ammo = new((2, 1), 5);
            Level level = new(
                player: player,
                hippo: hippo,
                gravity: Gravity.Default,
                secondaryObjects: new() { ammo, new Wall(Color.Yellow, (2, 3)) });

            // Act and assert
            Assert.IsNotNull(level.Hippo);
            Assert.ThrowsException<NotSupportedException>(() => level.Replace(level.Hippo, ammo));
        }

        [TestMethod]
        public void CannotReplaceNonHippoWithHippo()
        {
            // Arrange
            Player player = Player.Default(1, 1);
           
            Ammo ammo = new((2, 1), 5);
            Level level = new(
                player: player,
                hippo: null,
                gravity: Gravity.Default,
                secondaryObjects: new() { ammo, new Wall(Color.Yellow, (2, 3)) });
            Hippo hippo = new((2, 2), 5, false);

            // Act and assert
            Assert.ThrowsException<NotSupportedException>(() => level.Replace(ammo, hippo));
        }

        [TestMethod]
        public void ReplacePlayer_Test()
        {
            // Arrange
            Player p1 = Player.Default(1, 1);
            Level level = new(
                    player: p1,
                    hippo: null,
                    gravity: Gravity.None,
                    secondaryObjects: new()
                    {
                        new Wall(Color.Yellow, (1,2)),
                        new Wall(Color.Yellow, (2,2)),
                        new Wall(Color.Yellow, (3,2))
                    }
                );
            Player p2 = Player.Default(2, 1);
            // Act
            Level fromReplace = level.Replace(p1, p2);
            Level fromWithPlayer = level.WithPlayer(p2);
            // Assert
            Assert.AreEqual(new Location(2,1), fromReplace.Player.Location);
            Assert.AreEqual(new Location(2, 1), fromWithPlayer.Player.Location);
            Assert.AreEqual(fromReplace, fromWithPlayer);
        }

        [TestMethod]
        public void ReplaceHippo_Test()
        {
            // Arrange
            Hippo h1 = new((2, 1), 5, false);
            Level level = new(
                    player: Player.Default(1, 1),
                    hippo: h1,
                    gravity: Gravity.Default,
                    secondaryObjects: new()
                    {
                        new Wall(Color.Yellow, (1,2)),
                        new Wall(Color.Yellow, (2,2)),
                        new Wall(Color.Yellow, (3,2))
                    }
                );
            Hippo h2 = new((3, 1), 5, false);
            // Act
            Level fromReplace = level.Replace(h1, h2);
            // Assert
            Assert.IsNotNull(fromReplace.Hippo);
            Assert.AreEqual(new Location(3, 1), fromReplace.Hippo.Location);
        }

        [TestMethod]
        public void ReplaceSecondaryObject_Test()
        {
            // Arrange
            Ammo ammo = new((2, 1), 5);
            Level level = new(
                player: Player.Default(1, 1),
                hippo: new((2, 2), 5, false),
                gravity: Gravity.Default,
                secondaryObjects: new()
                {
                    new Wall(Color.Yellow, (2, 3)),
                    ammo
                });
            Bullet bullet = new(new Location(3, 1), Direction.West);
            Assert.IsTrue(level.Contains(ammo));
            Assert.IsFalse(level.Contains(bullet));
            
            // Act
            level = level.Replace(ammo, bullet);
            // Assert
            Assert.IsFalse(level.Contains(ammo));
            Assert.IsTrue(level.Contains(bullet));
        }

        [TestMethod]
        public void HashCodesEqual_EqualPlayers_EqualHippos_EqualObjects_EqualGravity_Test()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Hippo hippo = new((2, 1), 5, false);
            Wall w1 = new(Color.Yellow, (1, 2));
            Wall w2 = new(Color.Yellow, (2, 2));
            Wall w3 = new(Color.Yellow, (3, 2));

            Level l1 = new(
                player: player,
                hippo: hippo,
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w3 });
            Level l2 = new(
                player: player,
                hippo: hippo,
                gravity: Gravity.Default,
                secondaryObjects: new() { w3, w2, w1 }); // same objects, different order

            // Act
            int hash1 = l1.GetHashCode();
            int hash2 = l2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }


        [TestMethod]
        public void HashCodesNotEqual_EqualPlayers_EqualHippos_EqualObjects_GravityNotEqual_Test()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Hippo hippo = new((2, 1), 5, false);
            Wall w1 = new(Color.Yellow, (1, 2));
            Wall w2 = new(Color.Yellow, (2, 2));
            Wall w3 = new(Color.Yellow, (3, 2));

            Level l1 = new(
                player: player,
                hippo: hippo,
                gravity: new Gravity(Strength: 1, WaitCycles: 1),
                secondaryObjects: new() { w1, w2, w3 });
            Level l2 = new(
                player: player,
                hippo: hippo,
                gravity: new Gravity(Strength: 1, WaitCycles: 2),
                secondaryObjects: new() { w3, w2, w1 }); // same objects, different order

            // Act
            int hash1 = l1.GetHashCode();
            int hash2 = l2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashCodesNotEqual_EqualPlayers_EqualHippos_ObjectsNotEqual_GravityEqual_Test()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Hippo hippo = new((2, 1), 5, false);
            Wall w1 = new(Color.Yellow, (1, 2));
            Wall w2 = new(Color.Yellow, (2, 2));
            Wall w3 = new(Color.Yellow, (3, 2));
            Wall w4 = new(Color.Yellow, (4, 2));

            Level l1 = new(
                player: player,
                hippo: hippo,
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w3 });
            Level l2 = new(
                player: player,
                hippo: hippo,
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w4 });

            // Act
            int hash1 = l1.GetHashCode();
            int hash2 = l2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashCodesNotEqual_PlayersNotEqual_EqualHippos_ObjectsEqual_GravityEqual_Test()
        {
            // Arrange
            Hippo hippo = new((2, 1), 5, false);
            Wall w1 = new(Color.Yellow, (1, 2));
            Wall w2 = new(Color.Yellow, (2, 2));
            Wall w3 = new(Color.Yellow, (3, 2));

            Level l1 = new(
                player: Player.Default(1,1),
                hippo: hippo,
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w3 });
            Level l2 = new(
                player: Player.Default(3,1),
                hippo: hippo,
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w3 });

            // Act
            int hash1 = l1.GetHashCode();
            int hash2 = l2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashCodesNotEqual_PlayersEqual_HipposNotEqual_ObjectsEqual_GravityEqual_Test()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Hippo h1 = new((2, 1), 5, false);
            Hippo h2 = new((3, 1), 5, false);
            Wall w1 = new(Color.Yellow, (1, 2));
            Wall w2 = new(Color.Yellow, (2, 2));
            Wall w3 = new(Color.Yellow, (3, 2));

            Level l1 = new(
                player: player,
                hippo: h1,
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w3 });
            Level l2 = new(
                player: player,
                hippo: h2,
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w3 });

            // Act
            int hash1 = l1.GetHashCode();
            int hash2 = l2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashCodesNotEqual_PlayersEqual_OneHippoNullOtherNotNull_ObjectsEqual_GravityEqual_Test()
        {
            // Arrange
            Player player = Player.Default(1, 1);
            Wall w1 = new(Color.Yellow, (1, 2));
            Wall w2 = new(Color.Yellow, (2, 2));
            Wall w3 = new(Color.Yellow, (3, 2));

            Level l1 = new(
                player: player,
                hippo: new((2, 1), 5, false),
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w3 });
            Level l2 = new(
                player: player,
                hippo: null,
                gravity: Gravity.Default,
                secondaryObjects: new() { w1, w2, w3 });

            // Act
            int hash1 = l1.GetHashCode();
            int hash2 = l2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }
    }
}
