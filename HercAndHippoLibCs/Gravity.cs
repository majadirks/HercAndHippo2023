namespace HercAndHippoLibCs;
public record Gravity(int Strength, int WaitCycles)
{
    public static implicit operator Gravity(int strength) => new(Strength: Math.Max(strength, 0), WaitCycles: 1);
    public static implicit operator int(Gravity gravity) => gravity.Strength;
}