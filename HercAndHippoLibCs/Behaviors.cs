namespace HercAndHippoLibCs
{
    public static class Behaviors
    {
        public static Level DieAndStopBullet(HercAndHippoObj shot, Level level, Bullet shotBy) => level.Without(shot).Without(shotBy);
        public static Level NoReaction(Level level) => level;
        public static Level StopBullet(Level level, Bullet shotBy) => level.Without(shotBy);
        public static Level AllowBulletToPass<T>(T shot, Level level, Bullet shotBy) where T:HercAndHippoObj, ILocatable
            => level.Replace(shotBy, shotBy with { Location = shot.Location });
        public static Level Die<T>(Level level, T toDie) where T : HercAndHippoObj, ILocatable
            => level.Without(toDie);
    }

}
