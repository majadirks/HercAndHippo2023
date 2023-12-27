using static System.Math;

namespace HercAndHippoLibCs;

public record AmmoCount
{
    private const int MIN_AMMO = 0;
    private const int DEFAULT_STARTING_AMMO = 0;
    private int AmmoAmt { get; init; }
    public AmmoCount(int ammo = DEFAULT_STARTING_AMMO) => AmmoAmt = Max(MIN_AMMO, ammo);
    public bool HasAmmo => AmmoAmt > 0;
    public static AmmoCount operator -(AmmoCount ammo, int subtrahend) => Max(MIN_AMMO, ammo.AmmoAmt - subtrahend);
    public static AmmoCount operator +(AmmoCount ammo, int addend) => ammo.AmmoAmt + addend;
    public static implicit operator AmmoCount(int ammo) => new(ammo);
    public static implicit operator int(AmmoCount ac) => ac.AmmoAmt;
    public override string ToString() => $"Ammo Count: {AmmoAmt}";
}
