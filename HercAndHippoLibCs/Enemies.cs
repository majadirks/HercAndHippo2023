namespace HercAndHippoLibCs
{
    public record Bullet(Location Location, Direction Whither) : IDisplayable, ICyclable
    {
        public string ConsoleDisplayString => "*";
        public ConsoleColor Color => ConsoleColor.White;
        /// <summary>When the level cycles, a bullet moves in the direction it's currently heading.</summary>
        public Level Cycle(Level curState, ConsoleKeyInfo keyInfo)
        {
            Level nextState = curState
                .ObjectsAt(Location)
                .Where(obj => obj is IShootable shot)
                .Cast<IShootable>()
                .Aggregate(seed: curState, func: (state, shot) => shot.OnShot(state, shotFrom: Whither.Mirror(), shotBy: this));

            // Continue moving in current direction if it hasn't been stopped
            Level bulletMoved = nextState.Displayables.Contains(this) ?
                nextState.Replace(this, this with { Location = NextLocation }) : // If bullet wasn't stopped, continue
                nextState; // If bullet was stopped, don't regenerate it

            // If reached screen boundary, die 
            if (Location.Row == Row.MinRow || Location.Row == Row.MaxRow || Location.Col == Column.MinCol || Location.Col == Column.MaxCol)
                bulletMoved = bulletMoved.Without(this);
            return bulletMoved;
        }

        private Location NextLocation
            => Whither switch
            {
                Direction.North => Location with { Row = Location.Row - 1 },
                Direction.South => Location with { Row = Location.Row + 1 },
                Direction.East => Location with { Col = Location.Col + 1 },
                Direction.West => Location with { Col = Location.Col - 1 },
                Direction.Seek => throw new NotImplementedException(), // todo
                Direction.Flee => throw new NotImplementedException(), // todo
                _ => throw new NotImplementedException()
            };
    }


}
