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

    #region HercAndHippoObjCollection. No good reason to use this. Commenting out for now, will probably delete in the future.
    //public class HercAndHippoObjCollection : IEnumerable<HercAndHippoObj>, IEquatable<HercAndHippoObjCollection>
    //{
    //    private readonly HashSet<HercAndHippoObj> wrappedHashSet;
    //    public HercAndHippoObjCollection(HashSet<HercAndHippoObj> collection) 
    //    {
    //        this.wrappedHashSet = collection;
    //    }
    //    public HercAndHippoObjCollection AddObject(HercAndHippoObj toAdd)
    //    {
    //        HashSet<HercAndHippoObj> withAddition = new(wrappedHashSet) { toAdd };
    //        return new HercAndHippoObjCollection(withAddition);
    //    }

    //    public bool Equals(HercAndHippoObjCollection? other)
    //       => wrappedHashSet.GetHashCode() == other?.wrappedHashSet.GetHashCode() &&
    //           wrappedHashSet.Zip(other.wrappedHashSet).All(zipped => zipped.First.Equals(zipped.Second));

    //    public override bool Equals(object? obj) => obj is HercAndHippoObjCollection other && this.Equals(other);

    //    public override int GetHashCode()
    //    {
    //        // Adapted from code by Jon Skeet:
    //        // https://stackoverflow.com/questions/8094867/good-gethashcode-override-for-list-of-foo-objects-respecting-the-order
    //        unchecked
    //        {
    //            return 19 + wrappedHashSet.Select(takeable => 31 + takeable.GetHashCode()).Sum();
    //        }
    //    }

    //    public IEnumerator<HercAndHippoObj> GetEnumerator() => wrappedHashSet.GetEnumerator();

    //    public HercAndHippoObjCollection RemoveObject(HercAndHippoObj toRemove)
    //    {
    //        HashSet<HercAndHippoObj> removed = new(wrappedHashSet);
    //        removed.Remove(toRemove);
    //        return new(removed);
    //    }

    //    IEnumerator IEnumerable.GetEnumerator() => wrappedHashSet.GetEnumerator();

    //}
    #endregion
}
