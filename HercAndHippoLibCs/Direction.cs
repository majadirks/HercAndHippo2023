namespace HercAndHippoLibCs;

public enum Direction { Idle, North, East, South, West, Seek, Flee}
public static class DirectionExtensions
{
    
    public static Direction Mirror(this Direction toMirror)
        => toMirror switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            Direction.Idle => Direction.Idle,
            Direction.Seek => Direction.Flee,
            Direction.Flee => Direction.Seek,
            _ => throw new NotSupportedException()
        };

    public static Direction Flee<T>(this T hho, Level level) where T : HercAndHippoObj, ILocatable
        => hho.Seek(level, out int _).Mirror();
}