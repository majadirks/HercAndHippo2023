namespace HercAndHippoLibCs
{
    public record Bullet(Location Location, Direction Whither) : IDisplayable, ICyclable
    {
        public string ConsoleDisplayString => "*";
        public ConsoleColor Color => ConsoleColor.White;
        /// <summary>When the level cycles, a bullet moves in the direction it's currently heading.</summary>
        public Level Cycle(Level curState, ConsoleKeyInfo keyInfo)
            => curState.ObjectAt(NextLocation) switch
                {
                    // If there is an IShootable in the new location, call its OnShot method
                    IShootable shootableAtLocation => shootableAtLocation.OnShot(curState, shotFrom: Whither.Mirror(), shotBy: this).Without(this),
                    // Otherwise bullet keeps moving
                    _ => curState.Without(this).AddObject(this with { Location = NextLocation })
                };

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
}
