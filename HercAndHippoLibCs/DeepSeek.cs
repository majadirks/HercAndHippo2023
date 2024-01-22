using static HercAndHippoLibCs.Location;

namespace HercAndHippoLibCs;

public record DeepSeekResults(Direction Direction, Location Location, int Metric); 

public static class DeepSeekExtensions
{
    private record DeepSeekParams(HercAndHippoObj Hho, Level Level, int Depth, Location CameFrom);
    private static readonly Dictionary<DeepSeekParams, DeepSeekResults> deepSeekCache = new();

    /// <summary>>
    /// Similar to seek, but recursively attempts to minimize 
    /// distance to player after (lookahead) moves ahead
    /// </summary>
    public static DeepSeekResults DeepSeek<T>(this T hho, Level level, int depth, Location cameFrom) where T : HercAndHippoObj, ILocatable
        => DeepSeek<T>(new(hho, level, depth, cameFrom));

    private static DeepSeekResults DeepSeek<T>(DeepSeekParams ssps) where T : HercAndHippoObj, ILocatable
    {
        if (deepSeekCache.TryGetValue(ssps, out DeepSeekResults? value) && value != null)
        {
            return value;
        }

        T hho = (T)ssps.Hho;
        Level level = ssps.Level;
        int depth = ssps.Depth;
        Player player = level.Player;
        Location cameFrom = ssps.CameFrom;
        int initialDistance = ManhattanDistance(hho.Location, player.Location);
        if (hho.Location == player.Location)
        {
            return new DeepSeekResults(Direction.Idle, hho.Location, 0);
        }
        else if (depth == 0)
        {
            return new DeepSeekResults(Direction.Idle, hho.Location, initialDistance);
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
            Level newLevel = level.ReplaceIfPresent(hho, newHho);
            var northResults = DeepSeek(newHho, newLevel, depth - 1, cameFrom: hho.Location);
            northDist = northResults.Metric;
        }
        if (!hho.MotionBlockedTo(level, Direction.East) && cameFrom != nextEast)
        {
            T newHho = hho with { Location = nextEast };
            Level newLevel = level.ReplaceIfPresent(hho, newHho);
            var eastResults = DeepSeek(newHho, newLevel, depth - 1, cameFrom: hho.Location);
            eastDist = eastResults.Metric;
        }
        if (!hho.MotionBlockedTo(level, Direction.South) && cameFrom != nextSouth)
        {
            T newHho = hho with { Location = nextSouth };
            Level newLevel = level.ReplaceIfPresent(hho, newHho);
            var southResults = DeepSeek(newHho, newLevel, depth - 1, cameFrom: hho.Location);
            southDist = southResults.Metric;
        }
        if (!hho.MotionBlockedTo(level, Direction.West) && cameFrom != nextWest)
        {
            T newHho = hho with { Location = nextWest };
            Level newLevel = level.ReplaceIfPresent(hho, newHho);
            var westResults = DeepSeek(newHho, newLevel, depth - 1, cameFrom: hho.Location);
            westDist = westResults.Metric;
        }

        int[] distances = new int[] { northDist, eastDist, southDist, westDist };
        int newDist = distances.Min();
        Location newLoc = hho.Location;
        Direction newDir = Direction.Idle;
        if (newDist == int.MaxValue || newDist == initialDistance)
        {
            newLoc = hho.Location;
            newDir = Direction.Idle;
        }
        else if (newDist == northDist)
        {
            newLoc = nextNorth;
            newDir = Direction.North;
        }
        else if (newDist == eastDist)
        {
            newLoc = nextEast;
            newDir = Direction.East;
        }
        else if (newDist == southDist)
        {
            newLoc = nextSouth;
            newDir = Direction.South;
        }
        else if (newDist == westDist)
        {
            newLoc = nextWest;
            newDir = Direction.West;
        }
        else
            throw new NotSupportedException($"An unexpected error occurred in method {nameof(DeepSeek)}.");
        DeepSeekResults results = new(Direction: newDir, Location: newLoc, Metric: newDist);

        deepSeekCache.Add(ssps, results);
        return results;
    }
}
