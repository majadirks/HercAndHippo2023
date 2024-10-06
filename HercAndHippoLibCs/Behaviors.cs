namespace HercAndHippoLibCs;
public static class Behaviors
{      
    public static Level NoReaction(this Level level) => level;    
    public static Level Die<T>(this T toDie, Level level) where T : HercAndHippoObj, ILocatable
    {
        if (toDie is Player player)
        {
            Player nextPlayer = player with { Health = 0 };
            return level.WithPlayer(nextPlayer);
        }
        else
        {
            return level.Without(toDie);
        }
    }

    public static Level ApplyGravity<T>(this T toFall, Level level) where T : HercAndHippoObj, ILocatable
    {
        if (!level.GravityApplies())
            return level;
        Level nextState = level;

        for (int i = 0;
            i < level.Gravity.Strength &&
            !toFall.MotionBlockedTo(level, Direction.South);
            i++)
        {
            nextState = toFall.TryMoveSouth(nextState);
        }

        // If reached southernmost row, die (fall into the abyss)
        nextState = FallIntoAbyssAtBottomRow(nextState, toFall);

        return nextState;
    }

    public static Level WrapAroundTorusFromBottomRow<T>(this T toFall, Level level) where T : HercAndHippoObj, ILocatable
    {
        if (toFall.Location.Row != level.Height) 
            return level;
        Location newLocation = new(toFall.Location.Col, 1);
        if (level.ObjectsAt(newLocation).Where(bl => bl.BlocksMotion(level, toFall)).Any())
            return level;
        return level.ReplaceIfPresent(toFall, toFall with { Location = newLocation });
    }

    public static Level FallIntoAbyssAtBottomRow<T>(Level level, T toDie) where T : HercAndHippoObj, ILocatable
    {
        if (toDie.Location.Row == level.Height)
            return toDie.Die(level);
        else
            return level.NoReaction();
    }

    private static Level TryMoveSouth<T>(this T toFall, Level level) where T : HercAndHippoObj, ILocatable
    {
        if (toFall.MotionBlockedTo(level, Direction.South))
            return level.NoReaction();
        Location nextLocation = new(toFall.Location.Col, toFall.Location.Row.NextSouth(level.Height));
        return level.ReplaceIfPresent(toFall, toFall with { Location = nextLocation});
    }

    /// <summary>
    /// Trigger the <see cref="OnTouch"/> method for the toucher 
    /// and for any touchables at the location
    /// </summary>
    /// <returns>The state resulting from all the <see cref="OnTouch"/> calls</returns>
    public static Level MutualTouch<T>(this T toucher, Level level, Location location, Direction touchFrom) where T : HercAndHippoObj, ILocatable, ITouchable
    {
        Level nextLevel = level;
        var touchables = nextLevel
                   .ObjectsAt(location)
                   .Where(obj => obj.IsTouchable)
                   .Cast<ITouchable>()
                   .ToList();
        // Call touch methods for any touchables at nextEast
        nextLevel = touchables
            .Aggregate(
            seed: nextLevel,
            func: (state, touchable) => touchable.OnTouch(state, touchFrom, toucher));
        // Call Groodle OnTouch methods for any touchables at nextWest
        nextLevel = touchables
            .Aggregate(
            seed: nextLevel,
            func: (state, touchable) => toucher.OnTouch(state, touchFrom.Mirror(), touchable));
        return nextLevel;
    }
}
