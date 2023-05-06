using System.Runtime.CompilerServices;
using static System.Math;

namespace HercAndHippoLibCs
{
    public record Column : IComparable<Column>
    {
        private readonly int colNum;
        public Column(int col) => colNum = Min(Max(0, col), Console.BufferWidth - 3);
        public static implicit operator Column(int col) => new(col);
        public static implicit operator int(Column col) => col.colNum;
        public override string ToString() => $"{colNum}";
        public int CompareTo(Column? other) => colNum.CompareTo(other?.colNum);
    }
    public record Row : IComparable<Row>
    {
        private readonly int rowNum;
        public Row(int row) => rowNum = Min(Max(0, row), Console.BufferHeight - 3);
        public static implicit operator Row(int row) => new(row);
        public static implicit operator int(Row row) => row.rowNum;
        public override string ToString() => $"{rowNum}";
        public int CompareTo(Row? other) => rowNum.CompareTo(other?.rowNum);
    }

    public record Location(Column Col, Row Row)
    {
        public static implicit operator Location((int col, int row) tuple) => new(tuple.col, tuple.row);
        public override string ToString() => $"(col {Col}, row {Row})";
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
