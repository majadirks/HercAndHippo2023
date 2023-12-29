namespace HercAndHippoLibCs;
public static class Behaviors
{      
    public static Level NoReaction(Level level) => level;
    public static Level AllowBulletToPass<T>(T shot, Level level, Bullet shotBy) where T:HercAndHippoObj, ILocatable
        => level.Replace(shotBy, shotBy with { Location = shot.Location });
    public static Level Die<T>(Level level, T toDie) where T : HercAndHippoObj, ILocatable
        => level.Without(toDie);
}
