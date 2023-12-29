using System;
namespace HercAndHippoLibCs;

public static class MotionBlockChecker
{
    public static bool MotionBlockedTo(this ILocatable obj, Level level, Direction where)
        => where switch
        {
            Direction.North => MotionBlockedNorth(obj.Location, level),
            Direction.East => MotionBlockedEast(obj.Location, level),
            Direction.South => MotionBlockedSouth(obj.Location, level),
            Direction.West => MotionBlockedWest(obj.Location, level),
            _ => false
        };
    private static bool MotionBlockedEast(Location location, Level level)
    {
        if (location.Col == level.Width) return true;
        Column nextEast = location.Col.NextEast(level.Width);
        Location eastLoc = (nextEast, location.Row);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(eastLoc);
        return blockers.Where(bl => bl.BlocksMotion(level)).Any();
    }
    private static bool MotionBlockedWest(Location location, Level level)
    {
        if (location.Col == Column.MIN_COL) return true;
        Column nextWest = location.Col.NextWest();
        Location westLoc = (nextWest, location.Row);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(westLoc);
        return blockers.Where(bl => bl.BlocksMotion(level)).Any();
    }

    private static bool MotionBlockedNorth(Location location, Level level)
    {
        if (location.Row == Row.MIN_ROW) return true;
        Row nextNorth = location.Row.NextNorth();
        Location northLoc = (location.Col, nextNorth);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(northLoc);
        return blockers.Where(bl => bl.BlocksMotion(level)).Any();
    }

    private static bool MotionBlockedSouth(Location location, Level level)
    {
        if (location.Row == level.Height) return true;
        Row nextSouth = location.Row.NextSouth(level.Height);
        Location southLoc = (location.Col, nextSouth);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(southLoc);
        return blockers.Where(bl => bl.BlocksMotion(level)).Any();
    }
}
