using System.Reflection.Emit;

namespace HercAndHippoLibCs
{
    public record Bullet(Location Location, Direction Whither) : IDisplayable, ICyclable
    {
        public ConsoleColor Color => ConsoleColor.White;

        /// <summary>
        /// When the level cycles, bullet moves in the direction it's currently heading.
        /// </summary>
        public Level Cycle(Level curState, ConsoleKeyInfo keyInfo)
        {
            Bullet nextBullet = this with { Location = NextLocation };
            Level newState = curState.ObjectAt(NextLocation) switch
            {
                // If there is an IShootable in the new location, call its OnShot method
                IShootable shootableAtLocation => shootableAtLocation.OnShot(curState, shotFrom: Whither.Mirror(), shotBy: this).Without(this),
                // Otherwise bullet keeps moving
                _ => curState.Without(this).AddObject(nextBullet)
            };
            return newState;
        }

        private Location NextLocation => Whither switch
        {
            Direction.North => this.Location.With(row: this.Location.Row - 1),
            Direction.South => this.Location.With(row: this.Location.Row + 1),
            Direction.East => this.Location.With(col: this.Location.Col + 1),
            Direction.West => this.Location.With(col: this.Location.Col - 1),
            Direction.Seek => throw new NotImplementedException(), // todo
            Direction.Flee => throw new NotImplementedException(), // todo
            _ => throw new NotImplementedException()
        };
        

        public string ConsoleDisplayString => "*";

    }
}
