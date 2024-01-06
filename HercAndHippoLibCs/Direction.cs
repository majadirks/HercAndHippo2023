using static HercAndHippoLibCs.Location;
namespace HercAndHippoLibCs;

public enum Direction { Idle, North, East, South, West, Seek, Flee}
public static class DirectionExtensions
{
    
    public static Direction Mirror(this Direction toMirror)
        => toMirror switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            Direction.Idle => Direction.Idle,
            Direction.Seek => Direction.Flee,
            Direction.Flee => Direction.Seek,
            _ => throw new NotSupportedException()
        };
    /// <summary>
    /// If the given object is unable to move, this method returns Direction.Idle.
    /// If the given object is able to move but its legal moves to not bring it closer
    /// to the Player, this method returns Direction.Idle;
    /// Otherwise, it returns a direction in which the object is able to move
    /// and which brings it closer to the player,
    /// as measured using the Manhattan Distance / Taxi Cab Metric.
    /// </summary>
    public static Direction Seek<T>(this T hho, Level level, ILocatable toSeek, out int newDist) where T : HercAndHippoObj, ILocatable
    {
        int initialDistance = ManhattanDistance(hho.Location, toSeek.Location);
        if (hho.Location == toSeek.Location || hho is Player)
        {
            newDist = 0;
            return Direction.Idle;
        }

        Location nextNorth = new(hho.Location.Col, hho.Location.Row.NextNorth());
        Location nextEast = new(hho.Location.Col.NextEast(level.Width), hho.Location.Row);
        Location nextSouth = new(hho.Location.Col, hho.Location.Row.NextSouth(level.Height));
        Location nextWest = new(hho.Location.Col.NextWest(), hho.Location.Row);

        int northDist = hho.MotionBlockedTo(level, Direction.North) ? int.MaxValue : ManhattanDistance(nextNorth, toSeek.Location);
        int eastDist = hho.MotionBlockedTo(level, Direction.East) ? int.MaxValue : ManhattanDistance(nextEast, toSeek.Location);
        int southDist = hho.MotionBlockedTo(level, Direction.South) ? int.MaxValue : ManhattanDistance(nextSouth, toSeek.Location);
        int westDist = hho.MotionBlockedTo(level, Direction.West) ? int.MaxValue : ManhattanDistance(nextWest, toSeek.Location);

        int[] distances = new int[] { northDist, eastDist, southDist, westDist };
        newDist = distances.Min();
        if (newDist == int.MaxValue || newDist == initialDistance)
            return Direction.Idle;
        else if (newDist == northDist)
            return Direction.North;
        else if (newDist == eastDist)
            return Direction.East;
        else if (newDist == southDist)
            return Direction.South;
        else if (newDist == westDist)
            return Direction.West;
        else
            throw new NotSupportedException($"An unexpected error occurred in method {nameof(Seek)}.");
    }
    public static Direction Flee<T>(this T hho, Level level, ILocatable toFlee) where T : HercAndHippoObj, ILocatable
        => hho.Seek(level, toFlee, out int _).Mirror();
}