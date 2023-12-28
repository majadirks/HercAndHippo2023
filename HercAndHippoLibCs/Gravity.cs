namespace HercAndHippoLibCs;
public record Gravity(int Strength, int WaitCycles)
{
    public static readonly Gravity None = new(Strength: 0, WaitCycles: 1);
    public static readonly Gravity Default = new(Strength:1, WaitCycles: 2);
}