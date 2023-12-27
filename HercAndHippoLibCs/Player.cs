﻿namespace HercAndHippoLibCs;

public record Player : HercAndHippoObj, ILocatable, IShootable, ICyclable, ITouchable, IConsoleDisplayable
{
    public Player(Location location, Health health, AmmoCount ammoCount, Inventory inventory, int jumpStrength = 5, int kineticEnergy = 0)
    {
        Location = location;
        Health = health;
        AmmoCount = ammoCount;
        Inventory = inventory;
        Velocity = 0;
        JumpStrength = Math.Max(0, jumpStrength);
        KineticEnergy = Math.Max(0, kineticEnergy);
    }

    // Public properies
    public Location Location { get; init; }
    public Health Health { get; init; }
    public AmmoCount AmmoCount { get; init; }
    public Inventory Inventory { get; init; }
    public Velocity Velocity { get; init; }
    public int JumpStrength { get; init; }
    public int KineticEnergy { get; init; }
    public string ConsoleDisplayString => HasHealth ? "☻" : "X";
    public Color Color => Color.White;
    public Color BackgroundColor => Color.Blue;
    public bool HasHealth => Health.HasHealth;
    public bool HasAmmo => AmmoCount.HasAmmo;
    public override string ToString() => $"Player at location {Location} with {Health}, {AmmoCount}, Inventory Size: {Inventory.Count}";
    
    // Behaviors
    public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => level.WithPlayer(this with { Health = Health - 5 });
    public Level Cycle(Level level, ActionInput actionInput)
    {
        Velocity nextVelocity = Velocity.NextVelocity(this, level, actionInput);
        Level nextState = level.WithPlayer(this with { Velocity = nextVelocity });

        // Move east/west if velocity is >= 1 or if there was a moveEast/moveWest input
        if (Velocity <= -1)
        {
            for (int i = -1; i >= Velocity; i--)
            {
                Column nextWest = nextState.Player.Location.Col.NextWest();
                nextState = TryMoveTo((nextWest, Location.Row), approachFrom: Direction.East, curState: nextState);
            }
        }
        else if (Velocity >= 1)
        {
            for (int i = 1; i <= Velocity; i++)
            {
                Column nextEast = nextState.Player.Location.Col.NextEast(nextState.Width);
                nextState = TryMoveTo((nextEast, Location.Row), approachFrom: Direction.West, curState: nextState);
            }              
        }
        else if (actionInput == ActionInput.MoveWest)
        {
            Column nextWest = nextState.Player.Location.Col.NextWest();
            nextState = TryMoveTo((nextWest, Location.Row), approachFrom: Direction.East, curState: nextState);
        }
        else if (actionInput == ActionInput.MoveEast)
        {
            Column nextEast = nextState.Player.Location.Col.NextEast(nextState.Width);
            nextState = TryMoveTo((nextEast, Location.Row), approachFrom: Direction.West, curState: nextState);
        }
        
        // Regardless of above motion, run the following switch statement
        // to make sure we can shoot while velocity is nonzero
        return actionInput switch // Based on input, move north/south or shoot.
        {      
            ActionInput.MoveNorth => TryMoveTo((Location.Col, Location.Row.NextNorth()), approachFrom: Direction.South, curState: nextState),
            ActionInput.MoveSouth => TryMoveTo((Location.Col, Location.Row.NextSouth(level.Height)), approachFrom: Direction.North, curState: nextState),
            ActionInput.ShootNorth => Shoot(nextState, Direction.North),
            ActionInput.ShootSouth => Shoot(nextState, Direction.South),
            ActionInput.ShootWest => Shoot(nextState, Direction.West),
            ActionInput.ShootEast => Shoot(nextState, Direction.East),
            _ => Behaviors.NoReaction(nextState)
        };
    }
    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
        => touchedBy switch
        {
            Bullet shotBy => OnShot(level, touchedFrom.Mirror(), shotBy),
            _ => level
        };
    public override bool BlocksMotion(Player p) => p != this;

    // Check for blocking
    public bool MotionBlockedTo(Level level, Direction where)
            => where switch
            {
                Direction.North => MotionBlockedNorth(level),
                Direction.East => MotionBlockedEast(level),
                Direction.South => MotionBlockedSouth(level),
                Direction.West => MotionBlockedWest(level),
                _ => false
            };
    private bool MotionBlockedEast(Level level)
    {
        if (Location.Col == level.Width) return true;
        Column nextEast = Location.Col.NextEast(level.Width);
        Location eastLoc = (nextEast, Location.Row);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(eastLoc);
        return blockers.Where(bl => bl.BlocksMotion(this)).Any();
    }
    private bool MotionBlockedWest(Level level)
    {
        if (Location.Col == Column.MIN_COL) return true;
        Column nextWest = Location.Col.NextWest();
        Location westLoc = (nextWest, Location.Row);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(westLoc);
        return blockers.Where(bl => bl.BlocksMotion(this)).Any();
    }

    private bool MotionBlockedNorth(Level level)
    {
        if (Location.Row == Row.MIN_ROW) return true;
        Row nextNorth = Location.Row.NextNorth();
        Location northLoc = (Location.Col, nextNorth);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(northLoc);
        return blockers.Where(bl => bl.BlocksMotion(this)).Any();
    }

    private bool MotionBlockedSouth(Level level)
    {
        if (Location.Row == level.Height) return true;
        Row nextSouth = Location.Row.NextSouth(level.Height);
        Location southLoc = (Location.Col, nextSouth);
        IEnumerable<HercAndHippoObj> blockers = level.ObjectsAt(southLoc);
        return blockers.Where(bl => bl.BlocksMotion(this)).Any();
    }

    // Private static helpers
    private static Level TryMoveTo(Location newLocation, Direction approachFrom, Level curState)
    {
        Player player = curState.Player;
        // If no obstacles, move
        if (!player.ObjectLocatedTo(curState, approachFrom.Mirror()))
            return curState.WithPlayer(player with { Location = newLocation });

        // Otherwise, call the touch methods for any ITouchables and move over all else
        Level nextState = curState;
        // The ObjectsAt() method returns ILocatable objects, so the following cast is safe.
        foreach (ILocatable obj in curState.ObjectsAt(newLocation).Cast<ILocatable>())
        {
            nextState = obj switch
            {
                ITouchable touchableAtLocation => touchableAtLocation.OnTouch(curState, approachFrom, player),
                _ => nextState.WithPlayer(player with { Location = newLocation })
            };
        }
        return nextState; 
    }
    private static Level Shoot(Level level, Direction whither)
    {
        Player player = level.Player;
        if (!player.HasAmmo) return level;
        int col = player.Location.Col;
        int row = player.Location.Row;
        int bulletStartCol = whither switch
        {
            Direction.East => col + 1,
            Direction.West => col - 1,
            Direction.North => col,
            Direction.South => col,
            _ => throw new NotImplementedException(),
        };
        int bulletStartRow = whither switch
        {
            Direction.North => row - 1,
            Direction.South => row + 1,
            Direction.East => row,
            Direction.West => row,
            _ => throw new NotImplementedException()
        };

        level = level
            .AddObject(new Bullet((bulletStartCol, bulletStartRow), whither))
            .WithPlayer(player with { AmmoCount = player.AmmoCount - 1 });
        return level;
    }

    // Inventory Management
    public Player Take(ITakeable toTake) => this with { Inventory = Inventory.AddItem(toTake) };
    public bool Has<T>(Color color) => Inventory.Contains<T>(color);
    public (bool dropped, ITakeable? item, Player newPlayerState) DropItem<T>(Color color)
    {
        (bool dropped, ITakeable? item, Inventory reducedInventory) = Inventory.DropItem<T>(color);
        return (dropped, item, this with { Inventory = reducedInventory });
    }

    // Public static utilities
    public static Player Default(Location location) => new(location: location, health: 100, ammoCount: 0, inventory: Inventory.EmptyInventory, jumpStrength: 5, kineticEnergy: 0);
    public static Player Default(Column col, Row row) => Player.Default((col, row));
}
