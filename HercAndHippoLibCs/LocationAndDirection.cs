using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Math;

namespace HercAndHippoLibCs
{
    public record Column : IComparable<Column>
    {
        public const int MIN_COL = 1;
        private readonly int colNum;
        public Column(int col) => colNum = Max(MIN_COL, col);
        public static implicit operator Column(int col) => new(col);
        public static implicit operator int(Column col) => col.colNum;
        public override string ToString() => $"{colNum}";
        public int CompareTo(Column? other) => colNum.CompareTo(other?.colNum);

        public Column NextEast(int levelWidth) => Min(colNum + 1, levelWidth);
        public Column NextWest() => Max(MIN_COL, colNum - 1);
    }
    public record Row : IComparable<Row>
    {
        public const int MIN_ROW = 1;

        private readonly int rowNum;
        public Row(int row) => rowNum = Max(MIN_ROW, row);
        public static implicit operator Row(int row) => new(row);
        public static implicit operator int(Row row) => row.rowNum;
        public override string ToString() => $"{rowNum}";
        public int CompareTo(Row? other) => rowNum.CompareTo(other?.rowNum);

        public Row NextNorth() => Max(MIN_ROW, rowNum - 1);
        public Row NextSouth(int levelHeight) => Min(rowNum + 1, levelHeight);
    }

    public record Location(Column Col, Row Row)
    {
        public static implicit operator Location((int col, int row) tuple) => new(tuple.col, tuple.row);
        public override string ToString() => $"(col {Col}, row {Row})";
    }
    public enum Direction { Idle, North, East, South, West, Seek, Flee }
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
