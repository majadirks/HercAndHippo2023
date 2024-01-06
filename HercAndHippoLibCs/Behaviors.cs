namespace HercAndHippoLibCs;
public static class Behaviors
{      
    public static Level NoReaction(Level level) => level;
    public static Level AllowBulletToPass<T>(T shot, Level level, Bullet shotBy) where T:HercAndHippoObj, ILocatable
        => level.Replace(shotBy, shotBy with { Location = shot.Location });
    public static Level Die<T>(Level level, T toDie) where T : HercAndHippoObj, ILocatable
        => level.Without(toDie);

    public static Level ApplyGravity<T>(Level level, T toFall) where T : HercAndHippoObj, ILocatable
    {
        if (!level.GravityApplies())
            return level;
        Level nextState = level;

        for (int i = 0;
            i < level.Gravity.Strength &&
            !toFall.MotionBlockedTo(level, Direction.South);
            i++)
        {
            nextState = TryMoveSouth(nextState, toFall);
        }

        // If reached southernmost row, die (fall into the abyss)
        nextState = FallIntoAbyssAtBottomRow(nextState, toFall);

        return nextState;
    }

    public static Level FallIntoAbyssAtBottomRow<T>(Level level, T toDie) where T : HercAndHippoObj, ILocatable
    {
        if (toDie.Location.Row == level.Height)
            return Behaviors.Die(level, toDie);
        else
            return Behaviors.NoReaction(level);
    }

    private static Level TryMoveSouth<T>(Level level, T toFall) where T : HercAndHippoObj, ILocatable
    {
        if (toFall.MotionBlockedTo(level, Direction.South))
            return Behaviors.NoReaction(level);
        Location nextLocation = new(toFall.Location.Col, toFall.Location.Row.NextSouth(level.Height));
        return level.Replace(toFall, toFall with { Location = nextLocation});
    }
}
