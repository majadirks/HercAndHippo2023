namespace HercAndHippoLibCs
{
    public static class Behaviors
    {
        public static bool BoundaryReached(this Level level, IDisplayable obj)
            => obj.Location.Row <= Row.MIN_ROW || obj.Location.Row > level.Height ||
               obj.Location.Col <= Column.MIN_COL || obj.Location.Col > level.Width;
        public static Level DieAndStopBullet(IDisplayable shot, Level level, Bullet shotBy) => level.Without(shot).Without(shotBy);
        public static Level NoReaction(Level level) => level;
        public static Level StopBullet(Level level, Bullet shotBy) => level.Without(shotBy);
        public static Level AllowBulletToPass(IDisplayable shot, Level level, Bullet shotBy)
            => level.Replace(shotBy, shotBy with { Location = shot.Location });
        public static Level DieAndAllowPassage(Level level, IDisplayable passedOver, Player passerOver)
            => level.Without(passedOver).WithPlayer(passerOver with { Location = passedOver.Location });
    }

}
