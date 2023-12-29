namespace HercAndHippoLibCs;

public enum Direction { Idle, North, East, South, West, Seek, Flee }

public static class DirectionExtensions
{
    public static Direction Mirror(this Direction toMirror)
        => toMirror switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            Direction.Seek => Direction.Flee,
            Direction.Flee => Direction.Seek,
            Direction.Idle => Direction.Idle,
            _ => throw new NotSupportedException()
        };
}