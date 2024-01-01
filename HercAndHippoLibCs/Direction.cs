using static HercAndHippoLibCs.Location;
namespace HercAndHippoLibCs;

public enum Direction { Idle, North, East, South, West, Seek, Flee}
public static class DirectionExtensions
{
    private record DeepSeekParams(HercAndHippoObj Hho, Level Level, int Lookahead, Location CameFrom);
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
    public static Location DeepSeek<T>(this T hho, Level level, int lookahead, out int newDist, Location cameFrom) where T : HercAndHippoObj, ILocatable
        => DeepSeek<T>(new(hho, level, lookahead, cameFrom), out newDist);

    private static Location DeepSeek<T>(DeepSeekParams ssps, out int newDist) where T : HercAndHippoObj, ILocatable
    {
        if (deepSeekCache.TryGetValue(ssps, out (Location, int) value))
        {
            newDist = value.Item2;
            return value.Item1;
        }

        T hho = (T)ssps.Hho;
        Level level = ssps.Level;
        int lookahead = ssps.Lookahead;
        Player player = level.Player;
        Location cameFrom = ssps.CameFrom;
        int initialDistance = ManhattanDistance(hho.Location, player.Location);
        if (hho.Location == player.Location)
        {
            newDist = 0;
            return hho.Location;
        }
        else if (lookahead == 0)
        {
            newDist = initialDistance;
            return hho.Location;
        }
        Location nextNorth = new(hho.Location.Col, hho.Location.Row.NextNorth());
        Location nextEast = new(hho.Location.Col.NextEast(level.Width), hho.Location.Row);
        Location nextSouth = new(hho.Location.Col, hho.Location.Row.NextSouth(level.Height));
        Location nextWest = new(hho.Location.Col.NextWest(), hho.Location.Row);

        int northDist = int.MaxValue;
        int eastDist = int.MaxValue;
        int southDist = int.MaxValue;
        int westDist = int.MaxValue;

        if (!hho.MotionBlockedTo(level, Direction.North) && cameFrom != nextNorth)
        {
            T newHho = hho with { Location = nextNorth };
            Level newLevel = level.Replace(hho, newHho);
            DeepSeek(newHho, newLevel, lookahead - 1, out int dist, cameFrom: hho.Location);
            northDist = dist;
        }
        if (!hho.MotionBlockedTo(level, Direction.East) && cameFrom != nextEast)
        {
            T newHho = hho with { Location = nextEast };
            Level newLevel = level.Replace(hho, newHho);
            DeepSeek(newHho, newLevel, lookahead - 1, out int dist, cameFrom: hho.Location);
            eastDist = dist;
        }
        if (!hho.MotionBlockedTo(level, Direction.South) && cameFrom != nextSouth)
        {
            T newHho = hho with { Location = nextSouth };
            Level newLevel = level.Replace(hho, newHho);
            DeepSeek(newHho, newLevel, lookahead - 1, out int dist, cameFrom: hho.Location);
            southDist = dist;
        }
        if (!hho.MotionBlockedTo(level, Direction.West) && cameFrom != nextWest)
        {
            T newHho = hho with { Location = nextWest };
            Level newLevel = level.Replace(hho, newHho);
            DeepSeek(newHho, newLevel, lookahead - 1, out int dist, cameFrom: hho.Location);
            westDist = dist;
        }

        int[] distances = new int[] { northDist, eastDist, southDist, westDist };
        newDist = distances.Min();
        Location newLoc = hho.Location;
        if (newDist == int.MaxValue || newDist == initialDistance)
            newLoc = hho.Location;
        else if (newDist == northDist)
            newLoc = nextNorth;
        else if (newDist == eastDist)
            newLoc = nextEast;
        else if (newDist == southDist)
            newLoc = nextSouth;
        else if (newDist == westDist)
            newLoc = nextWest;
        else
            throw new NotSupportedException($"An unexpected error occurred in method {nameof(DeepSeek)}.");

        deepSeekCache.Add(ssps, (newLoc, newDist));
        return newLoc;
    }

    private static readonly Dictionary<DeepSeekParams, (Location, int)> deepSeekCache = new();

    public static Direction Flee<T>(this T hho, Level level) where T : HercAndHippoObj, ILocatable
        => hho.Seek(level, out int _).Mirror();
}