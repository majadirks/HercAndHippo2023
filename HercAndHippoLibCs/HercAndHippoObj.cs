﻿namespace HercAndHippoLibCs
{
    public abstract record HercAndHippoObj 
    {
        public HercAndHippoObj()
        {
            thisIsLocatable =  this is ILocatable;
            thisIsTouchable = this is ITouchable;
            thisIsShootable = this is IShootable;
        }
        public bool IsLocatable => thisIsLocatable;
        public bool IsTouchable => thisIsTouchable;
        public bool IsShootable => thisIsShootable;
        private readonly bool thisIsLocatable;
        private readonly bool thisIsTouchable;
        private readonly bool thisIsShootable;
        public abstract bool BlocksMotion(Player p);

        public bool ObjectLocatedTo(Level level, Direction where)
            => this is ILocatable locatable && 
            where switch
            {
                Direction.North => ObjectLocatedNorth(level, locatable),
                Direction.East => ObjectLocatedEast(level, locatable),
                Direction.South => ObjectLocatedSouth(level, locatable),
                Direction.West => ObjectLocatedWest(level, locatable),
                _ => false
            };  
        private static bool ObjectLocatedEast(Level level, ILocatable locatable)
        {
            if (locatable.Location.Col == level.Width) return true;
            Column nextEast = locatable.Location.Col.NextEast(level.Width);
            Location eastLoc = (nextEast, locatable.Location.Row);
            IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(eastLoc);
            return blockers.Where(bl => bl.IsLocatable && !bl.Equals(locatable)).Any();
        }
        private static bool ObjectLocatedWest(Level level, ILocatable locatable)
        {
            if (locatable.Location.Col == Column.MIN_COL) return true;
            Column nextWest = locatable.Location.Col.NextWest();
            Location westLoc = (nextWest, locatable.Location.Row);
            IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(westLoc);
            return blockers.Where(bl => bl.IsLocatable && !bl.Equals(locatable)).Any();
        }
        private static bool ObjectLocatedNorth(Level level, ILocatable locatable)
        {
            if (locatable.Location.Row == Row.MIN_ROW) return true;
            Row nextNorth = locatable.Location.Row.NextNorth();
            Location northLoc = (locatable.Location.Col, nextNorth);
            IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(northLoc);
            return blockers.Where(bl => bl.IsLocatable && !bl.Equals(locatable)).Any();
        }
        private static bool ObjectLocatedSouth(Level level, ILocatable locatable)
        {
            if (locatable.Location.Row == level.Height) return true;
            Row nextSouth = locatable.Location.Row.NextSouth(level.Height);
            Location southLoc = (locatable.Location.Col, nextSouth);
            IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(southLoc);
            return blockers.Where(bl => bl.IsLocatable && !bl.Equals(locatable)).Any();
        }

    }
}
