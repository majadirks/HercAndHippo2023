namespace HercAndHippoLibCs;

/// <summary>
/// The slowness of an enemy is the number of cycles between each movement.
/// </summary>
public record Slowness(int CyclesPerMovement)
{
    public const int MAX_SPEED = 10;
    public int Speed => MAX_SPEED - CyclesPerMovement;
    public bool Applies(Level level) => level.Cycles % CyclesPerMovement == 0;

    public static implicit operator Slowness(int cpm) 
        => cpm > 0 && cpm < MAX_SPEED ? 
        new Slowness(cpm) : 
        throw new ArgumentOutOfRangeException($"Cycles per movement must be in 0..{MAX_SPEED}, but was given {cpm}");
    public static implicit operator int(Slowness sp) => sp.CyclesPerMovement;
}
