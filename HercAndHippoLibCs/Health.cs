using static System.Math;

namespace HercAndHippoLibCs;

public record Health : IComparable<Health>
{
    private const int MIN_HEALTH = 0;
    private const int MAX_HEALTH = 200;
    private const int DEFAULT_STARTING_HEALTH = 100;
    private int HealthAmt { get; init; }
    public Health(int health = DEFAULT_STARTING_HEALTH) => HealthAmt = Min(Max(health, MIN_HEALTH), MAX_HEALTH);
    public bool HasHealth => HealthAmt > 0;
    public static Health operator -(Health health, int subtrahend) => Max(MIN_HEALTH, health.HealthAmt - subtrahend);
    public static Health operator +(Health health,int addend) => Min(MAX_HEALTH, health.HealthAmt + addend);
    public static bool operator <(Health a, Health b) => a.HealthAmt < b.HealthAmt;
    public static bool operator >(Health a, Health b) => a.HealthAmt > b.HealthAmt;
    public static implicit operator Health(int health) => new(health);
    public override string ToString() => $"Health: {HealthAmt}";
    public int CompareTo(Health? other)
    {
        if (other == null) throw new NullReferenceException();
        return this.HealthAmt.CompareTo(other.HealthAmt);
    }
}
