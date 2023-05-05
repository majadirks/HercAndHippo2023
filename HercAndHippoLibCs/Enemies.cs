namespace HercAndHippoLibCs
{
    public record Bullet(Location Location, Direction Whither) : IDisplayable, ITouchable
    {
        public Color Color => Color.White;

        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
            => touchedBy is IShootable shootable ?
            shootable.OnShot(level, shotFrom: touchedFrom.Mirror(), shotBy: this).Without(this) :
            level.Without(this);

    }
}
