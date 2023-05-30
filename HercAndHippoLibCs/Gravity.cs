namespace HercAndHippoLibCs
{
    public readonly struct Gravity
    {
        public Gravity(float gravConstant)
        {
            this.GravitationalConstant = gravConstant;
        }
        public static implicit operator Gravity(float g) => new(gravConstant: g);
        public static implicit operator float(Gravity grav) => grav.GravitationalConstant;
        public readonly float GravitationalConstant { get; init; }

        public Location NextLocation<T>(Level level, T hho) where T: HercAndHippoObj, ILocatable
        {
            if (hho == null) throw new ArgumentNullException(nameof(hho));
            if (!hho.AffectedByGravity || hho.IsBlocked(level, Direction.South)) return hho.Location;
            throw new NotImplementedException();
        }

    }
}
