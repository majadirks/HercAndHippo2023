using static System.Math;

namespace HercAndHippoLibCs
{
    public record Column : IComparable<Column>
    {
        public static int MinCol => 1;
        public static int MaxCol
        {
            get {
                    try { return Console.BufferWidth - 3; }
                    catch (IOException) { return 128; } // Console not available, eg when running test project
                }
        }
        private readonly int colNum;
        public Column(int col) => colNum = Min(Max(MinCol, col), MaxCol);
        public static implicit operator Column(int col) => new(col);
        public static implicit operator int(Column col) => col.colNum;
        public override string ToString() => $"{colNum}";
        public int CompareTo(Column? other) => colNum.CompareTo(other?.colNum);
    }
    public record Row : IComparable<Row>
    {
        public static int MinRow => 1;
        public static int MaxRow
        {
            get
            {
                try { return Console.BufferHeight - 3; }
                catch (IOException) { return 128; } // Console not available, eg when running test project
            }
        }
        private readonly int rowNum;
        public Row(int row) => rowNum = Min(Max(MinRow, row), MaxRow);
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
