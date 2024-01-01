using static HercAndHippoLibCs.Location;
namespace HercAndHippoLibCs;

public enum Direction { Idle, North, East, South, West}
public static class DirectionExtensions
{
    private record SuperSeekParams(dynamic Hho, Level Level, int Lookahead);
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

    /// <summary>
    /// If the given object is unable to move, this method returns Direction.Idle.
    /// If the given object is able to move but its legal moves to not bring it closer
    /// to the Player, this method returns Direction.Idle;
    /// Otherwise, it returns a direction in which the object is able to move
    /// and which brings it closer to the player,
    /// as measured using the Manhattan Distance / Taxi Cab Metric.
    /// </summary>
    public static Direction Seek<T>(this T hho, Level level, out int newDist) where T: HercAndHippoObj, ILocatable
    {
        Player player = level.Player;
        int initialDistance = ManhattanDistance(hho.Location, player.Location);
        if (hho.Location == player.Location || hho is Player)
        {
            newDist = 0;
            return Direction.Idle;
        }
                 
        Location nextNorth = new(hho.Location.Col, hho.Location.Row.NextNorth());
        Location nextEast = new(hho.Location.Col.NextEast(level.Width), hho.Location.Row);
        Location nextSouth = new(hho.Location.Col, hho.Location.Row.NextSouth(level.Height));
        Location nextWest = new(hho.Location.Col.NextWest(), hho.Location.Row);

        int northDist = hho.MotionBlockedTo(level, Direction.North) ? int.MaxValue : ManhattanDistance(nextNorth, player.Location);
        int eastDist = hho.MotionBlockedTo(level, Direction.East) ? int.MaxValue: ManhattanDistance(nextEast, player.Location);
        int southDist = hho.MotionBlockedTo(level, Direction.South) ? int.MaxValue : ManhattanDistance(nextSouth, player.Location);
        int westDist = hho.MotionBlockedTo(level, Direction.West) ? int.MaxValue : ManhattanDistance(nextWest,player.Location);

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

    /// <summary>>
    /// Similar to seek, but recursively attempts to minimize 
    /// distance to player after (lookahead) moves ahead
    /// </summary>
    public static Direction SuperSeek<T>(this T hho, Level level, int lookahead, out int newDist) where T : HercAndHippoObj, ILocatable
        => SuperSeek<T>(new(hho, level, lookahead), out newDist);

    private static Direction SuperSeek<T>(SuperSeekParams ssps, out int newDist) where T : HercAndHippoObj, ILocatable
    {
        if (superSeekCache.TryGetValue(ssps, out (Direction, int) value))
        {
            newDist = value.Item2;
            return value.Item1;
        }

        T hho = ssps.Hho;
        Level level = ssps.Level;
        int lookahead = ssps.Lookahead;
        Player player = level.Player;
        int initialDistance = ManhattanDistance(hho.Location, player.Location);
        if (lookahead < 1)
        {
            newDist = initialDistance;
            return Direction.Idle;
        }
        else if (lookahead == 1)
        {
            return Seek(hho, level, out newDist);
        }
        else if (hho.Location == player.Location || hho is Player)
        {
            newDist = 0;
            return Direction.Idle;
        }
        Location nextNorth = new(hho.Location.Col, hho.Location.Row.NextNorth());
        Location nextEast = new(hho.Location.Col.NextEast(level.Width), hho.Location.Row);
        Location nextSouth = new(hho.Location.Col, hho.Location.Row.NextSouth(level.Height));
        Location nextWest = new(hho.Location.Col.NextWest(), hho.Location.Row);

        int northDist = int.MaxValue;
        int eastDist = int.MaxValue;
        int southDist = int.MaxValue;
        int westDist = int.MaxValue;

        if (!hho.MotionBlockedTo(level, Direction.North))
        {
            T newHho = hho with { Location = nextNorth };
            Level newLevel = level.Replace(hho, newHho);
            SuperSeek(newHho, newLevel, lookahead - 1, out northDist);
        }
        if (!hho.MotionBlockedTo(level, Direction.East))
        {
            T newHho = hho with { Location = nextEast };
            Level newLevel = level.Replace(hho, newHho);
            SuperSeek(newHho, newLevel, lookahead - 1, out eastDist);
        }
        if (!hho.MotionBlockedTo(level, Direction.South))
        {
            T newHho = hho with { Location = nextSouth };
            Level newLevel = level.Replace(hho, newHho);
            SuperSeek(newHho, newLevel, lookahead - 1, out southDist);
        }
        if (!hho.MotionBlockedTo(level, Direction.West))
        {
            T newHho = hho with { Location = nextWest };
            Level newLevel = level.Replace(hho, newHho);
            SuperSeek(newHho, newLevel, lookahead - 1, out westDist);
        }

        int[] distances = new int[] { northDist, eastDist, southDist, westDist };
        newDist = distances.Min();
        Direction newDir = Direction.Idle;
        if (newDist == int.MaxValue || newDist == initialDistance)
            newDir = Direction.Idle;
        else if (newDist == northDist)
            newDir = Direction.North;
        else if (newDist == eastDist)
            newDir = Direction.East;
        else if (newDist == southDist)
            newDir = Direction.South;
        else if (newDist == westDist)
            newDir = Direction.West;
        else
            throw new NotSupportedException($"An unexpected error occurred in method {nameof(SuperSeek)}.");

        superSeekCache.Add(ssps, (newDir, newDist));
        return newDir;
    }

    private static readonly Dictionary<SuperSeekParams, (Direction, int)> superSeekCache = new();

    public static Direction Flee<T>(this T hho, Level level) where T : HercAndHippoObj, ILocatable
        => hho.Seek(level, out int _).Mirror();
}