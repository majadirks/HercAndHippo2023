using static System.Math;
namespace HercAndHippoLibCs
{
    public interface IDisplayable 
    { 
        public Location Location { get; } 
        public Color Color { get; }
    }
    public interface IShootable<T> { public Mortal<T> OnShot(Direction shotFrom); }
    public interface IColorful<T> { public Color Color { get; } }
    public readonly struct Location
    {
        public readonly int Col { get; init; }
        public readonly int Row { get; init; }
        public Location(int col, int row)  
            => (Col, Row) = (Min(Max(0,col), Console.BufferWidth - 1), Min(Max(0,row), Console.BufferHeight - 1));
  
        public static implicit operator Location((int col, int row) tuple) => new(tuple.col, tuple.row);
    }
    public enum Color { Red, Orange, Yellow, Green, Blue, Purple, Black, White }
    public enum Direction { North, East, South, West }
    public readonly struct EmptySpace { };
    public record Wall(Color Color, Location Location) : IDisplayable;

    public record BreakableWall(Color Color, Location Location) : IDisplayable, IShootable<BreakableWall>
    {
        public Mortal<BreakableWall> OnShot(Direction shotFrom) => new EmptySpace();
    }
    public record Door(Color Color, Location Location) : IDisplayable; // dies when correct key is used
}