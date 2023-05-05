using static System.Math;
namespace HercAndHippoLibCs
{
    public interface IDisplayable 
    { 
        public Location Location { get; } 
        public Color Color { get; }
    }
    public interface IShootable { public Level OnShot(Level level, Direction shotFrom); }
    public interface ITouchable { public Level OnTouch(Level level, Direction touchedFrom); };
    public readonly struct Location
    {
        public readonly int Col { get; init; }
        public readonly int Row { get; init; }
        public Location(int col, int row)  
            => (Col, Row) = (Min(Max(0,col), Console.BufferWidth - 3), Min(Max(0,row), Console.BufferHeight - 3));
  
        public static implicit operator Location((int col, int row) tuple) => new(tuple.col, tuple.row);
        public override string ToString() => $"(col {Col}, row {Row})";
    }
    public enum Color { Red, Yellow, Green, Blue, Purple, Black, White }
    public enum Direction { North, East, South, West }
    public readonly struct EmptySpace { };
    public record Wall(Color Color, Location Location) : IDisplayable, ITouchable
    {
        public Level OnTouch(Level level, Direction touchedFrom) => level;
    }

    public record BreakableWall(Color Color, Location Location) : IDisplayable, IShootable, ITouchable
    {
        public Level OnShot(Level level, Direction shotFrom) => level.Without(this);
        public Level OnTouch(Level level, Direction touchedFrom) => level;
    }
    public record Door(Color Color, Location Location) : IDisplayable; // TODO: dies when correct key is used
}