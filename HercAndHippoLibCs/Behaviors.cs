namespace HercAndHippoLibCs
{
    public static class Behaviors
    {
        public static Level DieAndStopBullet(ILocatable shot, Level level, Bullet shotBy) => level.Without(shot).Without(shotBy);
        public static Level NoReaction(Level level) => level;
        public static Level StopBullet(Level level, Bullet shotBy) => level.Without(shotBy);
        public static Level AllowBulletToPass(ILocatable shot, Level level, Bullet shotBy)
            => level.Replace(shotBy, shotBy with { Location = shot.Location });
        public static Level DieAndAllowPassage(Level level, ILocatable passedOver, Player passerOver)
            => level.Without(passedOver).WithPlayer(passerOver with { Location = passedOver.Location });
    }

}
