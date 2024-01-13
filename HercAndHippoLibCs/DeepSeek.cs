using System.Collections.Concurrent;
using static HercAndHippoLibCs.Location;

namespace HercAndHippoLibCs;

public record DeepSeekResults(Direction Direction, Location Location, int Metric); 

public static class DeepSeekExtensions
{
    private const int MAX_COST = 2_000_000;
    private record DeepSeekParams(HercAndHippoObj Hho, Level Level, ILocatable ToSeek, int Depth, Location CameFrom);
    private static readonly ConcurrentDictionary<(Location a, Location b, int depth), DeepSeekResults> deepSeekCache = new();

    /// <summary>>
    /// Similar to seek, but recursively attempts to minimize 
    /// distance to player after (lookahead) moves ahead
    /// </summary>
    public static DeepSeekResults DeepSeek<T>(this T hho, Level level, ILocatable toSeek, int depth, Location cameFrom) where T : HercAndHippoObj, ILocatable
        => DeepSeek<T>(new(hho, level, toSeek, depth, cameFrom));

    private static DeepSeekResults DeepSeek<T>(DeepSeekParams ssps) where T : HercAndHippoObj, ILocatable
    {
        T hho = (T)ssps.Hho;
        int depth = ssps.Depth;
        if (deepSeekCache.TryGetValue((hho.Location, ssps.ToSeek.Location, depth), out DeepSeekResults? value) && value != null)
        {
            return value;
        }
        Level level = ssps.Level;
        ILocatable toSeek = ssps.ToSeek;
        Location cameFrom = ssps.CameFrom;
        int initialDistance = ManhattanDistance(hho.Location, toSeek.Location);
        if (hho.Location == toSeek.Location)
        {
            var dsr = new DeepSeekResults(Direction.Idle, hho.Location, 0);
            deepSeekCache.TryAdd((hho.Location, toSeek.Location, depth), dsr);
            return dsr;
        }
        else if (depth == 0)
        {
            Direction dir = hho.Seek(level, toSeek, out int d);
            var dsr = new DeepSeekResults(dir, hho.Location, d);
            deepSeekCache.TryAdd((hho.Location, toSeek.Location, depth), dsr);
            return dsr;
        }
        Location nextNorth = new(hho.Location.Col, hho.Location.Row.NextNorth());
        Location nextEast = new(hho.Location.Col.NextEast(level.Width), hho.Location.Row);
        Location nextSouth = new(hho.Location.Col, hho.Location.Row.NextSouth(level.Height));
        Location nextWest = new(hho.Location.Col.NextWest(), hho.Location.Row);

        int northMetric = MAX_COST;
        int eastMetric = MAX_COST;
        int southMetric = 500;
        int westMetric = 500;

        if (!hho.MotionBlockedTo(level, Direction.North) && cameFrom != nextNorth)
        {
            T newHho = hho with { Location = nextNorth };
            Level newLevel = level.Replace(hho, newHho);
            var northResults = DeepSeek(newHho, newLevel, toSeek, depth - 1, cameFrom: hho.Location);
            northMetric = northResults.Metric;
        }
        if (!hho.MotionBlockedTo(level, Direction.East) && cameFrom != nextEast)
        {
            T newHho = hho with { Location = nextEast };
            Level newLevel = level.Replace(hho, newHho);
            var eastResults = DeepSeek(newHho, newLevel, toSeek, depth - 1, cameFrom: hho.Location);
            eastMetric = eastResults.Metric;
        }
        if (!hho.MotionBlockedTo(level, Direction.South) && cameFrom != nextSouth)
        {
            T newHho = hho with { Location = nextSouth };
            Level newLevel = level.Replace(hho, newHho);
            var southResults = DeepSeek(newHho, newLevel, toSeek, depth - 1, cameFrom: hho.Location);
            southMetric = southResults.Metric;
        }
        if (!hho.MotionBlockedTo(level, Direction.West) && cameFrom != nextWest)
        {
            T newHho = hho with { Location = nextWest };
            Level newLevel = level.Replace(hho, newHho);
            var westResults = DeepSeek(newHho, newLevel, toSeek, depth - 1, cameFrom: hho.Location);
            westMetric = westResults.Metric;
        }

        int[] distances = new int[] { northMetric, eastMetric, southMetric, westMetric };
        int newDist = distances.Min();
        Location newLoc = hho.Location;
        Direction newDir = Direction.Idle;
        if (newDist == int.MaxValue || newDist == initialDistance)
        {
            newLoc = hho.Location;
            newDir = Direction.Idle;
        }
        else if (newDist == northMetric)
        {
            newLoc = nextNorth;
            newDir = Direction.North;
        }
        else if (newDist == eastMetric)
        {
            newLoc = nextEast;
            newDir = Direction.East;
        }
        else if (newDist == southMetric)
        {
            newLoc = nextSouth;
            newDir = Direction.South;
        }
        else if (newDist == westMetric)
        {
            newLoc = nextWest;
            newDir = Direction.West;
        }
        else
            throw new NotSupportedException($"An unexpected error occurred in method {nameof(DeepSeek)}.");
        int metric = Math.Min(1 + 2 * newDist + 3 * depth, MAX_COST);
        DeepSeekResults results = new(Direction: newDir, Location: newLoc, Metric: metric);

        deepSeekCache.TryAdd((hho.Location, toSeek.Location, depth), results);
        return results;
    }
}
