using static HercAndHippoLibCs.Location;
namespace HercAndHippoLibCs;

public enum Direction { Idle, North, East, South, West}

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
            _ => throw new NotSupportedException()
        };

    public static Direction Seek<T>(this T hho, Level level) where T: HercAndHippoObj, ILocatable
    {
        Player player = level.Player;
        if (hho.Location == player.Location || hho is Player) 
            return Direction.Idle;
        int initialDistance = ManhattanDistance(hho.Location, player.Location);

        Location nextNorth = new(hho.Location.Col, hho.Location.Row.NextNorth());
        Location nextEast = new(hho.Location.Col.NextEast(level.Width), hho.Location.Row);
        Location nextSouth = new(hho.Location.Col, hho.Location.Row.NextSouth(level.Height));
        Location nextWest = new(hho.Location.Col.NextWest(), hho.Location.Row);

        int northDist = hho.MotionBlockedTo(level, Direction.North) ? int.MaxValue : ManhattanDistance(nextNorth, player.Location);
        int eastDist = hho.MotionBlockedTo(level, Direction.East) ? int.MaxValue: ManhattanDistance(nextEast, player.Location);
        int southDist = hho.MotionBlockedTo(level, Direction.South) ? int.MaxValue : ManhattanDistance(nextSouth, player.Location);
        int westDist = hho.MotionBlockedTo(level, Direction.West) ? int.MaxValue : ManhattanDistance(nextWest,player.Location);

        int[] distances = new int[] { northDist, eastDist, southDist, westDist };
        int min = distances.Min();
        if (min == int.MaxValue)
            return Direction.Idle;
        else if (min == northDist)
            return Direction.North;
        else if (min == eastDist)
            return Direction.East;
        else if (min == southDist)
            return Direction.South;
        else if (min == westDist)
            return Direction.West;
        else
            throw new NotSupportedException($"An unexpectederror occurred in method {nameof(Seek)}.");
    }

    public static Direction Flee<T>(this T hho, Level level) where T : HercAndHippoObj, ILocatable
        => hho.Seek(level).Mirror();
}