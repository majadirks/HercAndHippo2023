namespace HercAndHippoLibCs
{
    public record Bullet(Location Location, Direction Whither) : IDisplayable, ICyclable
    {
        public string ConsoleDisplayString => "*";
        public ConsoleColor Color => ConsoleColor.White;
        /// <summary>When the level cycles, a bullet moves in the direction it's currently heading.</summary>
        public Level Cycle(Level curState, ConsoleKeyInfo keyInfo)
        {
            Level nextState = curState;
            var objectsAt = curState.ObjectsAt(NextLocation);

            // If no obstacles, move
            if (!objectsAt.Any()) return curState.Without(this).AddObject(this with { Location = NextLocation });

            // Otherwise, call the OnShot method for any objects encountered, or keep moving if none is defined
            foreach (IDisplayable obj in objectsAt)
            {
                nextState = obj switch
                {
                    // If there is an IShootable in the new location, call its OnShot method
                    IShootable shootableAtLocation => shootableAtLocation.OnShot(curState, shotFrom: Whither.Mirror(), shotBy: this).Without(this),
                    // Otherwise bullet keeps moving
                    _ => nextState.Without(this).AddObject(this with { Location = NextLocation })
                };
            }
            return nextState;
        }


        private Location NextLocation 
            => Whither switch
                {
                    Direction.North => Location with { Row = Location.Row - 1 },
                    Direction.South => Location with { Row = Location.Row + 1},
                    Direction.East => Location with { Col = Location.Col + 1},
                    Direction.West => Location with { Col = Location.Col - 1},
                    Direction.Seek => throw new NotImplementedException(), // todo
                    Direction.Flee => throw new NotImplementedException(), // todo
                    _ => throw new NotImplementedException()
                };
    }

    public static class OnShotBehaviors
    {
        public static Level DieAndStopBullet(IDisplayable shot, Level level, Bullet shotBy) => level.Without(shot).Without(shotBy);
        public static Level StopBullet(Level level) => level;
        public static Level AllowBulletToPass(IDisplayable shot, Level level, Bullet shotBy)
            => level.Without(shotBy).AddObject(shotBy with { Location = shot.Location });
    }


}
