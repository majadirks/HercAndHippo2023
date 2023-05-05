namespace HercAndHippoLibCs
{
    public interface IDisplayable 
    { 
        public Location Location { get; } 
        public Color Color { get; }
    }
    public interface IShootable<T> { public Mortal<T> OnShot(Direction shotFrom); }
    public interface IColorful<T> { public Color Color { get; } }
    public record Location(int Col, int Row)
    {
        public static implicit operator Location((int col, int row) tuple) => new Location(tuple.col, tuple.row);
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