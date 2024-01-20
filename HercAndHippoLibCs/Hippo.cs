namespace HercAndHippoLibCs;

public record HippoMotionBlockages(bool BlockedNorth, bool BlockedEast, bool BlockedWest)
{
    public bool HippoBlocksTo(Direction dir) 
        => dir switch
           {
               Direction.North => BlockedNorth,
               Direction.East => BlockedEast,
               Direction.West => BlockedWest,
               _ => false
           };

    public static HippoMotionBlockages GetBlockages(Level level)
    {
        Hippo? hippo = level.Hippo;
        if (hippo == null) return new HippoMotionBlockages(false, false, false);
        bool blockedNorth = hippo.LockedToPlayer && hippo.MotionBlockedTo(level, Direction.North);
        bool blockedEast = hippo.LockedToPlayer && hippo.MotionBlockedTo(level, Direction.East);
        bool blockedWest = hippo.LockedToPlayer && hippo.MotionBlockedTo(level, Direction.West);
        return new(BlockedNorth: blockedNorth, BlockedEast: blockedEast, BlockedWest: blockedWest);
    }

}
public record Hippo(Location Location, Health Health, bool LockedToPlayer) : HercAndHippoObj, ILocatable, ITouchable, ICyclable, IShootable, IConsoleDisplayable
{
    public const int HEALTH_PENALTY_ON_SHOT = 5;
    public Color Color => Color.Magenta;

    public Color BackgroundColor => Color.DarkBlue;

    public string ConsoleDisplayString => "H";//"🦛";

    public bool StopsBullet => true;

    public override bool BlocksMotion(Level level, ILocatable toBlock)
    {
        // Do not block a player immediately above
        if (this.Below(level.Player.Location)) 
            return false;
        else if (level.Player.Below(this.Location))
            return this.MotionBlockedTo(level, Direction.North);
        else if (!CanBePickedUp(level))
            return true;
        Direction whither = this.Flee(level, toBlock);
        return this.MotionBlockedTo(level, whither);
    }

    public Level Cycle(Level level, ActionInput actionInput)
    {
        if (!Health.HasHealth)
            return this.Die(level).Lose();
        else if (LockedToPlayer && actionInput == ActionInput.DropHippo)
            return PutDown(level);
        else if (LockedToPlayer)
        {
            Level locked = LockAbovePlayer(level);
            return locked;
        }
        else if (level.GravityApplies()) // Not locked to player; fall due to gravity if relevant
        {
            Level nextState = level;
            for (int i = 0;
                i < level.Gravity.Strength &&
                !this.MotionBlockedTo(level, Direction.South);
                i++)
            {
                nextState = TryMoveSouth(nextState);
            }

            // If we are falling into the abyss, die, unless locked to a player,
            if (nextState.Hippo != null && !nextState.Hippo.LockedToPlayer)
            {
                nextState =
                    Behaviors.FallIntoAbyssAtBottomRow(nextState, nextState.Hippo);
                if (nextState.Hippo == null)
                    return nextState.Lose();
            }
            
            return nextState;
        }
        else
            return level.NoReaction();
    }

    private static Level TryMoveSouth(Level level) 
    {
        Hippo? hippo = level.Hippo ?? throw new NullReferenceException();
        if (hippo.MotionBlockedTo(level, Direction.South))
            return level.NoReaction();
        Location nextLocation = new(hippo.Location.Col, hippo.Location.Row.NextSouth(level.Height));
        return level.Replace(hippo, hippo with { Location = nextLocation, LockedToPlayer = nextLocation == level.Player.Location });
    }

    public static Level LockAbovePlayer(Level level)
    {
        Hippo? hippo = level.Hippo ?? throw new NullReferenceException();
        Player player = level.Player;
        Location nextLocation = new(Col: player.Location.Col, Row: player.Location.Row.NextNorth());
        Hippo lockedHippo = hippo with { Location = nextLocation, LockedToPlayer = true };
        Level withLockedHippo = level.Replace(hippo, lockedHippo);
        return  withLockedHippo;
    }

    private static Level PutDown(Level level)
    {
        Hippo? hippo = level.Hippo ?? throw new NullReferenceException();
        Player player = level.Player;
        // First, attempt to place East
        bool blockedEast = player.MotionBlockedTo(level,Direction.East);
        Location NECorner = new(player.Location.Col.NextEast(level.Width), player.Location.Row.NextNorth());
        bool blockedNE = level.ObjectsAt(NECorner).Where(obj => obj.BlocksMotion(level, level.Player)).Any();
        if (!blockedEast && !blockedNE)
        {
            Location nextLocation = new(Col: player.Location.Col.NextEast(level.Width), Row: player.Location.Row);
            Level nextState = level.Replace(hippo, hippo with { Location = nextLocation, LockedToPlayer = false });
            return nextState;
        }

        // If that didn't work, attempt to place West
        bool blockedWest = player.MotionBlockedTo(level, Direction.West);
        Location NWCorner = new(player.Location.Col.NextWest(), player.Location.Row.NextNorth());
        bool blockedNW = level.ObjectsAt(NWCorner).Where(obj => obj.BlocksMotion(level, level.Player)).Any();
        if (!blockedWest && !blockedNW)
        {
            Location nextLocation = new(Col: player.Location.Col.NextWest(), Row: player.Location.Row);
            Level nextState = level.Replace(hippo, hippo with { Location = nextLocation, LockedToPlayer = false });
            return nextState;
        }

        // Could not put down hippo
        return level.NoReaction();
    }

    public Level OnShot(Level level, Direction shotFrom, Bullet shotBy)
        => level.Without(shotBy).Replace(this, this with { Health = this.Health - HEALTH_PENALTY_ON_SHOT });

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) 
        => touchedBy is Player && !LockedToPlayer ? PickUp(level) : level.NoReaction();
    private static bool CanBePickedUp(Level level)
    {
        Hippo? hippo = level.Hippo ??  throw new NullReferenceException();
        Player player = level.Player;
        if (hippo.Below(player.Location))
            return true;
        bool cannotLift = player.MotionBlockedTo(level, Direction.North) ||
            hippo.MotionBlockedTo(level, Direction.North);
        return !cannotLift;
    }
    private static Level PickUp(Level level)
        => CanBePickedUp(level) ? 
        LockAbovePlayer(level)
        .AddSecondaryObject(new Message("You picked up the hippo!")) : 
        level.NoReaction();
}

