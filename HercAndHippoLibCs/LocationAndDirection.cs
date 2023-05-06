using static System.Math;

namespace HercAndHippoLibCs
{
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
}
