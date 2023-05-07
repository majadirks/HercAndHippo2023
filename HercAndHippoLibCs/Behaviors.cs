namespace HercAndHippoLibCs
{
    public static class Behaviors
    {
        public static Level DieAndStopBullet(IDisplayable shot, Level level, Bullet shotBy) => level.Without(shot).Without(shotBy);
        public static Level NoReaction(Level level) => level;
        public static Level StopBullet(Level level, Bullet shotBy) => level.Without(shotBy);
        public static Level AllowBulletToPass(IDisplayable shot, Level level, Bullet shotBy)
            => level.Without(shotBy).AddObject(shotBy with { Location = shot.Location });
        public static Level DieAndAllowPassage(Level level, IDisplayable passedOver, Player passerOver)
            => level.Without(passedOver).WithPlayer(passerOver with { Location = passedOver.Location });
    }

}
