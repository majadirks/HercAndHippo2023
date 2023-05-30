using System.Collections;
using System.Diagnostics.CodeAnalysis;
using static System.Math;

namespace HercAndHippoLibCs
{
    public record Player : HercAndHippoObj, ILocatable, IShootable, ICyclable, ITouchable, IConsoleDisplayable
    {
        public Player(Location location, Health health, AmmoCount ammoCount, Inventory inventory)
        {
            Location = location;
            Health = health;
            AmmoCount = ammoCount;
            Inventory = inventory;
            Velocity = 0;
        }

        // Properties
        public Location Location { get; init; }
        public Health Health { get; init; }
        public AmmoCount AmmoCount { get; init; }
        public Inventory Inventory { get; init; }
        public Velocity Velocity { get; init; }
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

        // Private static helpers
        private static Level TryMoveTo(Location newLocation, Direction approachFrom, Level curState)
        {
            Player player = curState.Player;
            // If no obstacles, move
            if (!player.IsBlocked(curState, approachFrom.Mirror()))
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
        public static Player Default(Location location) => new(location: location, health: 100, ammoCount: 0, inventory: Inventory.EmptyInventory);
        public static Player Default(Column col, Row row) => Player.Default((col, row));
    }

    public static class InventoryExtensions
    {
        ///<summary>Returns true if an ITakeable is of the given type and color</summary> 
        public static bool MatchesColor<T>(this ITakeable item, Color color) => item is T && item.Color == color;
        public static Inventory ToInventory(this IEnumerable<ITakeable> enumerable) => new(enumerable);
    }

    /// <summary>
    /// Wraps a HashSet, but performs equality check by comparing items in the set rather than
    /// by reference equality. This allows two player objects to be equal if they have the same items
    /// in their inventory.
    /// </summary>
    public readonly struct Inventory : IEnumerable<ITakeable>, IEquatable<Inventory>
    {
        private readonly HashSet<ITakeable> takeables;
        public static Inventory EmptyInventory { get; } = new();
        public Inventory() => takeables = new HashSet<ITakeable>();
        public Inventory(HashSet<ITakeable> takeables) => this.takeables = takeables;
        public Inventory(IEnumerable<ITakeable> takeables) => this.takeables = new(takeables.ToHashSet());
        public Inventory(ITakeable starterItem) => takeables = new HashSet<ITakeable>() { starterItem };
        public Inventory AddItem(ITakeable item)
        {
            HashSet<ITakeable> newSet = new(takeables) { item };
            return new Inventory(newSet);
        }
        public (bool dropped, ITakeable? item, Inventory newInventoryState) DropItem<T>(Color color)
        {
            ITakeable? item = takeables.Where(item => item.MatchesColor<T>(color)).FirstOrDefault();
            if (item == default) return (false, item, this);
            Inventory newState = this.Where(item => !item.MatchesColor<T>(color)).ToInventory();
            return (true, item, newState);
        }
        public bool Contains<T>(Color color) => takeables.Where(item => item.MatchesColor<T>(color)).Any();
        public IEnumerator<ITakeable> GetEnumerator() => takeables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => takeables.GetEnumerator();
        public override bool Equals([NotNullWhen(true)] object? obj)
         => obj != null && obj is Inventory other && this.ContainsSameItemsAs(other);
        public bool Equals(Inventory other) => this.ContainsSameItemsAs(other);
        private bool ContainsSameItemsAs(Inventory other)
            => takeables.IsSubsetOf(other.takeables) && other.takeables.IsSubsetOf(takeables);
        public static bool operator ==(Inventory left, Inventory right) => left.Equals(right);  
        public static bool operator !=(Inventory left, Inventory right) => !(left == right);
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (var takeable in takeables)
                {
                    hash ^= takeable.GetHashCode();
                }
                return hash;
            }
        }
        public int Count => takeables.Count;
    }

    public record Health
    {
        private const int MIN_HEALTH = 0;
        private const int MAX_HEALTH = 200;
        private const int DEFAULT_STARTING_HEALTH = 100;
        private int HealthAmt { get; init; }
        public Health(int health = DEFAULT_STARTING_HEALTH) => HealthAmt = Min(Max(health, MIN_HEALTH), MAX_HEALTH);
        public bool HasHealth => HealthAmt > 0;
        public static Health operator -(Health health, int subtrahend) => Max(MIN_HEALTH, health.HealthAmt - subtrahend);
        public static Health operator +(Health health,int addend) => Min(MAX_HEALTH, health.HealthAmt + addend);
        public static implicit operator Health(int health) => new(health);
        public override string ToString() => $"Health: {HealthAmt}";
    }

    public record AmmoCount
    {
        private const int MIN_AMMO = 0;
        private const int DEFAULT_STARTING_AMMO = 0;
        private int AmmoAmt { get; init; }
        public AmmoCount(int ammo = DEFAULT_STARTING_AMMO) => AmmoAmt = Max(MIN_AMMO, ammo);
        public bool HasAmmo => AmmoAmt > 0;
        public static AmmoCount operator -(AmmoCount ammo, int subtrahend) => Max(MIN_AMMO, ammo.AmmoAmt - subtrahend);
        public static AmmoCount operator +(AmmoCount ammo, int addend) => ammo.AmmoAmt + addend;
        public static implicit operator AmmoCount(int ammo) => new(ammo);
        public static implicit operator int(AmmoCount ac) => ac.AmmoAmt;
        public override string ToString() => $"Ammo Count: {AmmoAmt}";
    }

    public record Velocity
    {
        private const int MAX_VELOCITY = 2;
        private const int MIN_VELOCITY = -2;
        private const float ZERO_THRESHOLD = 0.1f;
        private const float ACCELERATION = 0.2f;
        public float CurrentVelocity { get; init; }
        public Velocity(float velocity)
        {
            CurrentVelocity = Min(Max(velocity, MIN_VELOCITY), MAX_VELOCITY);
            if (Abs(CurrentVelocity) <= ZERO_THRESHOLD || Sign(CurrentVelocity) != Sign(velocity))
                CurrentVelocity = 0;
        }
        public Velocity NextVelocity(HercAndHippoObj hho, Level level, ActionInput actionInput)
        {
            if (actionInput == ActionInput.MoveEast && hho.IsBlocked(level, Direction.East))
                return 0;
            else if (actionInput == ActionInput.MoveWest && hho.IsBlocked(level, Direction.West))
                return 0;
            else return actionInput switch
            {
                ActionInput.MoveEast => AccelerateEastward(),
                ActionInput.MoveWest => AccelerateWestward(),
                _ => SlowDown()
            };
        }
        private Velocity SlowDown()
        {
            if (CurrentVelocity > 0) return AccelerateWestward();
            else if (CurrentVelocity < 0) return AccelerateEastward();
            else return this;
        }
        private Velocity AccelerateEastward() => new(velocity: CurrentVelocity + ACCELERATION);
        private Velocity AccelerateWestward() => new(velocity: CurrentVelocity - ACCELERATION);

        public static implicit operator Velocity(float cv) => new(velocity: cv);
        public static implicit operator float(Velocity veloc) => veloc.CurrentVelocity;
    }

}
