namespace HercAndHippoLibCs
{
    public abstract record HercAndHippoObj 
    {
        public virtual bool IsBlocking => this is ILocatable;
        public bool IsBlocked(Level level, Direction where)
            => this is ILocatable locatable && 
            where switch
            {
                Direction.North => IsBlockedNorth(level, locatable),
                Direction.East => IsBlockedEast(level, locatable),
                Direction.South => IsBlockedSouth(level, locatable),
                Direction.West => IsBlockedWest(level, locatable),
                _ => false
            };  
        private static bool IsBlockedEast(Level level, ILocatable locatable)
        {
            if (locatable.Location.Col == level.Width) return true;
            Column nextEast = locatable.Location.Col.NextEast(level.Width);
            Location eastLoc = (nextEast, locatable.Location.Row);
            IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(eastLoc);
            return blockers.Where(bl => bl.IsBlocking && !bl.Equals(locatable)).Any();
        }
        private static bool IsBlockedWest(Level level, ILocatable locatable)
        {
            if (locatable.Location.Col == Column.MIN_COL) return true;
            Column nextWest = locatable.Location.Col.NextWest();
            Location westLoc = (nextWest, locatable.Location.Row);
            IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(westLoc);
            return blockers.Where(bl => bl.IsBlocking && !bl.Equals(locatable)).Any();
        }
        private static bool IsBlockedNorth(Level level, ILocatable locatable)
        {
            if (locatable.Location.Row == Row.MIN_ROW) return true;
            Row nextNorth = locatable.Location.Row.NextNorth();
            Location northLoc = (locatable.Location.Col, nextNorth);
            IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(northLoc);
            return blockers.Where(bl => bl.IsBlocking && !bl.Equals(locatable)).Any();
        }
        private static bool IsBlockedSouth(Level level, ILocatable locatable)
        {
            if (locatable.Location.Row == level.Height) return true;
            Row nextSouth = locatable.Location.Row.NextSouth(level.Height);
            Location southLoc = (locatable.Location.Col, nextSouth);
            IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(southLoc);
            return blockers.Where(bl => bl.IsBlocking && !bl.Equals(locatable)).Any();
        }
    }
}
