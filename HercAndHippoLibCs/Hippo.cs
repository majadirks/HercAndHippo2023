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

    public static HippoMotionBlockages GetBlockages(Hippo? hippo, Level level)
    {
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

    public override bool BlocksMotion(Level level)
    {
        // Do not block a player immediately above
        if (this.Below(level.Player.Location)) 
            return false;
        else if (level.Player.Below(this.Location))
            return this.MotionBlockedTo(level, Direction.North);
        else if (!CanBePickedUp(level))
            return true;
        Direction whither = this.Flee(level);
        return this.MotionBlockedTo(level, whither);
    }

    public Level Cycle(Level level, ActionInput actionInput)
    {
        if (!Health.HasHealth)
            return Behaviors.Die(level, this);
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
            return nextState;
        }
        else
            return Behaviors.NoReaction(level);
    }

    private Level TryMoveSouth(Level level) 
    {
        if (this.MotionBlockedTo(level, Direction.South))
            return Behaviors.NoReaction(level);
        Location nextLocation = new(Location.Col, Location.Row.NextSouth(level.Height));
        return level.Replace(this, this with { Location = nextLocation });
    }

    public Level LockAbovePlayer(Level level)
    {
        Player player = level.Player;
        Location nextLocation = new(Col: player.Location.Col, Row: player.Location.Row.NextNorth());
        Hippo lockedHippo = this with { Location = nextLocation, LockedToPlayer = true };
        Level withLockedHippo = level.Replace(this, lockedHippo);
        return  withLockedHippo;
    }

    private Level PutDown(Level level)
    {
        Player player = level.Player;
        // First, attempt to place East
        bool blockedEast = player.MotionBlockedTo(level,Direction.East);
        Location NECorner = new(player.Location.Col.NextEast(level.Width), player.Location.Row.NextNorth());
        bool blockedNE = level.ObjectsAt(NECorner).Where(obj => obj.BlocksMotion(level)).Any();
        if (!blockedEast && !blockedNE)
        {
            Location nextLocation = new(Col: player.Location.Col.NextEast(level.Width), Row: player.Location.Row);
            Level nextState = level.Replace(this, this with { Location = nextLocation, LockedToPlayer = false });
            return nextState;
        }

        // If that didn't work, attempt to place West
        bool blockedWest = player.MotionBlockedTo(level, Direction.West);
        Location NWCorner = new(player.Location.Col.NextWest(), player.Location.Row.NextNorth());
        bool blockedNW = level.ObjectsAt(NWCorner).Where(obj => obj.BlocksMotion(level)).Any();
        if (!blockedWest && !blockedNW)
        {
            Location nextLocation = new(Col: player.Location.Col.NextWest(), Row: player.Location.Row);
            Level nextState = level.Replace(this, this with { Location = nextLocation, LockedToPlayer = false });
            return nextState;
        }

        // Could not put down hippo
        return Behaviors.NoReaction(level);
    }

    public Level OnShot(Level level, Direction shotFrom, Bullet shotBy)
        => level.Without(shotBy).Replace(this, this with { Health = this.Health - HEALTH_PENALTY_ON_SHOT });

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => PickUp(level);
    private bool CanBePickedUp(Level level)
    {
        Player player = level.Player;
        if (this.Below(player.Location))
            return true;
        bool cannotLift = player.MotionBlockedTo(level, Direction.North) ||
            this.MotionBlockedTo(level, Direction.North);
        return !cannotLift;
    }
    private Level PickUp(Level level)
    {
        if (!CanBePickedUp(level))
            return Behaviors.NoReaction(level);
        else
        {
            Level locked = LockAbovePlayer(level);
            return locked;
        }
    }
}

