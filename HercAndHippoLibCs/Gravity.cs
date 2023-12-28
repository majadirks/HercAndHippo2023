namespace HercAndHippoLibCs;
public record Gravity(int Strength, int WaitCycles)
{
    public const int DEFAULT_WAIT_CYCLES = 2;
    public static implicit operator Gravity(int strength) => new(Strength: Math.Max(strength, 0), WaitCycles: DEFAULT_WAIT_CYCLES);
    public static implicit operator int(Gravity gravity) => gravity.Strength;  
}