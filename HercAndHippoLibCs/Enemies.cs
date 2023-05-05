namespace HercAndHippoLibCs
{
    public record Bullet(Location Location, Direction Whither) : IDisplayable, ITouchable, ICyclable
    {
        public Color Color => Color.White;

        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
            => touchedBy is IShootable shootable ?
            shootable.OnShot(level, shotFrom: touchedFrom.Mirror(), shotBy: this).Without(this) :
            level.Without(this);

        /// <summary>
        /// When the level cycles, bullet moves in the direction it's currently heading.
        /// </summary>
        public Level Cycle(Level level, ConsoleKeyInfo keyInfo)
         => level.AddObject(this with { Location = NextLocation }).Without(this);

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
