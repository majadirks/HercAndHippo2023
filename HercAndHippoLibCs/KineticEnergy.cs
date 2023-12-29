namespace HercAndHippoLibCs;

public readonly struct KineticEnergy
{
    private readonly int energy;
    public KineticEnergy(int energy) => this.energy = Math.Max(0, energy);
    public static implicit operator int(KineticEnergy kineticEnergy) => kineticEnergy.energy;
    public static implicit operator KineticEnergy(int energy) => new(energy);
    public static readonly KineticEnergy None = new(0);
}
