using static System.Math;

namespace HercAndHippoLibCs;

public record Velocity
{
    private const int MAX_VELOCITY = 2;
    private const int MIN_VELOCITY = -2;
    private const float ZERO_THRESHOLD = 0.1f;
    private const float ACCELERATION = 0.2f;
    public float CurrentVelocity { get; init; }
    public Velocity(float velocity)
    {
        CurrentVelocity = Min(Max(velocity, MIN_VELOCITY), MAX_VELOCITY);
        if (Abs(CurrentVelocity) <= ZERO_THRESHOLD || Sign(CurrentVelocity) != Sign(velocity))
            CurrentVelocity = 0;
    }
    public Velocity NextVelocity(HercAndHippoObj hho, Level level, ActionInput actionInput)
    {
        if (actionInput == ActionInput.MoveEast && hho.ObjectLocatedTo(level, Direction.East))
            return 0;
        else if (actionInput == ActionInput.MoveWest && hho.ObjectLocatedTo(level, Direction.West))
            return 0;
        else return actionInput switch
        {
            ActionInput.MoveEast => AccelerateEastward(),
            ActionInput.MoveWest => AccelerateWestward(),
            _ => SlowDown()
        };
    }
    private Velocity SlowDown()
    {
        if (CurrentVelocity > 0) return AccelerateWestward();
        else if (CurrentVelocity < 0) return AccelerateEastward();
        else return this;
    }
    private Velocity AccelerateEastward() => new(velocity: CurrentVelocity + ACCELERATION);
    private Velocity AccelerateWestward() => new(velocity: CurrentVelocity - ACCELERATION);

    public static implicit operator Velocity(float cv) => new(velocity: cv);
    public static implicit operator float(Velocity veloc) => veloc.CurrentVelocity;
}
