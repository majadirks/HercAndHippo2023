using System.Runtime.CompilerServices;
using static System.Math;
namespace HercAndHippoLibCs
{
    public interface IDisplayable
    {
        public Location Location { get; }
        public Color Color { get; }
    public string ConsoleDisplayString { get; }
    }
    public interface IShootable { public Level OnShot(Level level, Direction shotFrom, Bullet shotBy); }
    public interface ITouchable { public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy); };
    public readonly struct Location
    {
        public readonly int Col { get; init; }
        public readonly int Row { get; init; }
        public Location(int col, int row)
            => (Col, Row) = (Min(Max(0, col), Console.BufferWidth - 3), Min(Max(0, row), Console.BufferHeight - 3));

        public static implicit operator Location((int col, int row) tuple) => new(tuple.col, tuple.row);
        public override string ToString() => $"(col {Col}, row {Row})";
        public Location With(int? col = null, int? row = null) => new(col: col ?? Col, row: row ?? Row);
    }
    public enum Color { Red, Yellow, Green, Blue, Purple, Black, White }
    public enum Direction { North, East, South, West, Seek, Flee }
    public static class DirectionExtensions
    {
        public static Direction Mirror(this Direction toMirror)
            => toMirror switch
            {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                Direction.West => Direction.North,
                Direction.Seek => Direction.Flee,
                Direction.Flee => Direction.Seek,
                _ => throw new NotSupportedException()
            };

    }
    public readonly struct EmptySpace { };
    public record Wall(Color Color, Location Location) : IDisplayable, ITouchable
    {
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => level;
        public string ConsoleDisplayString => "█";
    }

    public record BreakableWall(Color Color, Location Location) : IDisplayable, IShootable, ITouchable
    {
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => level.Without(this).Without(shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => level;
        public string ConsoleDisplayString => "▓";
    }
    public record Door(Color Color, Location Location) : IDisplayable
    {
        // TODO: dies when correct key is used
        public string ConsoleDisplayString => "D";
    }; 
}