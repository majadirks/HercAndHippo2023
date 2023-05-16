namespace HercAndHippoLibCs
{
    public record Bullet(Location Location, Direction Whither) : IDisplayable, ICyclable
    {
        public string ConsoleDisplayString => "○";
        public ConsoleColor Color => ConsoleColor.White;
        public ConsoleColor BackgroundColor => ConsoleColor.Black;
        /// <summary>When the level cycles, a bullet moves in the direction it's currently heading.</summary>
        public Level Cycle(Level curState, ActionInput actionInput)
        {
            // Call OnShot methods for any IShootable at current location 
            Level nextState = curState
                .ObjectsAt(Location)
                .Where(obj => obj is IShootable shot)
                .Cast<IShootable>()
                .Aggregate(seed: curState, func: (state, shot) => shot.OnShot(state, shotFrom: Whither.Mirror(), shotBy: this));

            // Continue moving in current direction if it hasn't been stopped
            Bullet bulletAtNextPosition = this with { Location = NextLocation };
            Level bulletMoved = nextState.Contains(this) ?
                nextState.Replace(this, bulletAtNextPosition) : // If bullet wasn't stopped, continue
                nextState; // If bullet was stopped, don't regenerate it

            // If reached level boundary, die 
            if (bulletMoved.Contains(bulletAtNextPosition) && bulletAtNextPosition.ExceededBoundary(bulletMoved))
                bulletMoved = bulletMoved.Without(bulletAtNextPosition);
            return bulletMoved;
        }

        private Location NextLocation
            => Whither switch
            {
                Direction.North => Location with { Row = Location.Row - 1 },
                Direction.South => Location with { Row = Location.Row + 1 },
                Direction.East => Location with { Col = Location.Col + 1 },
                Direction.West => Location with { Col = Location.Col - 1 },
                Direction.Idle => Location,
                Direction.Seek => throw new NotImplementedException(), // todo
                Direction.Flee => throw new NotImplementedException(), // todo
                _ => throw new NotImplementedException()
            };

        private bool ExceededBoundary(Level level)
            => Location.Row <= Row.MIN_ROW || Location.Row > level.Height ||
               Location.Col <= Column.MIN_COL || Location.Col > level.Width;
    }


}
