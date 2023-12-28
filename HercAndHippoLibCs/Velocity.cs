using static System.Math;

namespace HercAndHippoLibCs;

public record Velocity
{
    private const int MAX_VELOCITY = 2;
    private const int MIN_VELOCITY = -2;
    private const float ZERO_THRESHOLD = 0.05f;
    private const float ACCELERATION = 0.2f;
    public float CurrentVelocity { get; init; }
    public Velocity(float velocity)
    {
        CurrentVelocity = Min(Max(velocity, MIN_VELOCITY), MAX_VELOCITY);
        if (Abs(CurrentVelocity) <= ZERO_THRESHOLD || Sign(CurrentVelocity) != Sign(velocity))
            CurrentVelocity = 0;
    }
    public Velocity NextVelocity(Player player, Level level, ActionInput actionInput)
    {
        bool applyFriction = player.MotionBlockedTo(level, Direction.South);
        if (actionInput == ActionInput.MoveEast && player.MotionBlockedTo(level, Direction.East))
            return 0;
        else if (actionInput == ActionInput.MoveWest && player.MotionBlockedTo(level, Direction.West))
            return 0;
        else return actionInput switch
        {
            ActionInput.MoveEast => AccelerateEastward(),
            ActionInput.MoveWest => AccelerateWestward(),
            ActionInput.MoveNorth => CurrentVelocity,
            _ => applyFriction ? SlowDown() : CurrentVelocity
        };
    }
    private Velocity SlowDown()
    {
        if (CurrentVelocity > 0) return AccelerateWestward();
        else if (CurrentVelocity < 0) return AccelerateEastward();
        else return this;
    }
    private Velocity AccelerateEastward(float accel = ACCELERATION) => new(velocity: CurrentVelocity + accel);
    private Velocity AccelerateWestward(float accel = ACCELERATION) => new(velocity: CurrentVelocity - accel);

    public static implicit operator Velocity(float cv) => new(velocity: cv);
    public static implicit operator float(Velocity veloc) => veloc.CurrentVelocity;
}
