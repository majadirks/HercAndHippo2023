namespace HercAndHippoLibCs;

public enum Direction { Idle, North, East, South, West}

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
            _ => throw new NotSupportedException()
        };

    public static Direction Seek(this HercAndHippoObj hho, Player player)
    {
        throw new NotImplementedException();
    }
}