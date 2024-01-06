using System;
namespace HercAndHippoLibCs;

public static class MotionBlockChecker
{
    public static bool MotionBlockedTo(this ILocatable obj, Level level, Direction where)
        => where switch
        {
            Direction.North => MotionBlockedNorth(obj, level),
            Direction.East => MotionBlockedEast(obj, level),
            Direction.South => MotionBlockedSouth(obj, level),
            Direction.West => MotionBlockedWest(obj, level),
            _ => false
        };
    private static bool MotionBlockedEast(ILocatable obj, Level level)
    {
        Location location = obj.Location;
        if (location.Col == level.Width) return true;
        Column nextEast = location.Col.NextEast(level.Width);
        Location eastLoc = (nextEast, location.Row);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(eastLoc);
        return blockers.Where(bl => bl.BlocksMotion(level, obj)).Any();
    }
    private static bool MotionBlockedWest(ILocatable obj, Level level)
    {
        Location location = obj.Location;
        if (location.Col == Column.MIN_COL) return true;
        Column nextWest = location.Col.NextWest();
        Location westLoc = (nextWest, location.Row);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(westLoc);
        return blockers.Where(bl => bl.BlocksMotion(level, obj)).Any();
    }

    private static bool MotionBlockedNorth(ILocatable obj, Level level)
    {
        Location location = obj.Location;
        if (location.Row == Row.MIN_ROW) return true;
        Row nextNorth = location.Row.NextNorth();
        Location northLoc = (location.Col, nextNorth);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(northLoc);
        return blockers.Where(bl => bl.BlocksMotion(level, obj)).Any();
    }

    private static bool MotionBlockedSouth(ILocatable obj, Level level)
    {
        Location location = obj.Location;
        if (location.Row == level.Height) return true;
        Row nextSouth = location.Row.NextSouth(level.Height);
        Location southLoc = (location.Col, nextSouth);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(southLoc);
        return blockers.Where(bl => bl.BlocksMotion(level, obj)).Any();
    }
}
