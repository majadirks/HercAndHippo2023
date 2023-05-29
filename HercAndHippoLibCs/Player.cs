using System.Collections;
using System.Diagnostics.CodeAnalysis;
using static System.Math;

namespace HercAndHippoLibCs
{
    public record Player(Location Location, Health Health, AmmoCount AmmoCount, Inventory Inventory) 
        : HercAndHippoObj, ILocatable, IShootable, ICyclable, ITouchable, IConsoleDisplayable
    {
        public static Player Default(Location location) => new(Location: location, Health: 100, AmmoCount: 0, Inventory: Inventory.EmptyInventory);
        public static Player Default(Column col, Row row) => Player.Default((col, row));
        public string ConsoleDisplayString => HasHealth ? "☻" : "RIP";
        public ConsoleColor Color => ConsoleColor.White;
        public ConsoleColor BackgroundColor => ConsoleColor.Blue;
        public override string ToString() => $"Player at location {Location} with {Health}, {AmmoCount}, Inventory Size: {Inventory.Count}";
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy)
            => level.WithPlayer(this with { Health = Health - 5 });
        public bool HasHealth => Health.HasHealth;
        public bool HasAmmo => AmmoCount.HasAmmo;
        public Level Cycle(Level level, ActionInput actionInput)
         => actionInput switch
         {
             ActionInput.MoveWest => TryMoveTo((Location.Col.NextWest(), Location.Row), approachFrom: Direction.East, curState: level),
             ActionInput.MoveEast => TryMoveTo((Location.Col.NextEast(level.Width), Location.Row), approachFrom: Direction.West, curState: level),
             ActionInput.MoveNorth => TryMoveTo((Location.Col, Location.Row.NextNorth()), approachFrom: Direction.South, curState: level),
             ActionInput.MoveSouth => TryMoveTo((Location.Col, Location.Row.NextSouth(level.Height)), approachFrom: Direction.North, curState: level),
             ActionInput.ShootNorth => Shoot(level, Direction.North),
             ActionInput.ShootSouth => Shoot(level, Direction.South),
             ActionInput.ShootWest => Shoot(level, Direction.West),
             ActionInput.ShootEast => Shoot(level, Direction.East),
             _ => Behaviors.NoReaction(level)
         };
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
            => touchedBy switch
            {
                Bullet shotBy => OnShot(level, touchedFrom.Mirror(), shotBy),
                _ => level
            };
        private Level TryMoveTo(Location newLocation, Direction approachFrom, Level curState)
        {
            // If no obstacles, move
            if (!IsBlocked(curState, approachFrom.Mirror()))
                return curState.WithPlayer(this with { Location = newLocation });

            // Otherwise, call the touch methods for any ITouchables and move over all else
            Level nextState = curState;
            foreach (ILocatable obj in curState.ObjectsAt(newLocation))
            {
                nextState = obj switch
                {
                    ITouchable touchableAtLocation => touchableAtLocation.OnTouch(curState, approachFrom, this),
                    _ => nextState.WithPlayer(this with { Location = newLocation })
                };
            }
            return nextState; 
        }
        private Level Shoot(Level level, Direction whither)
        {
            if (!HasAmmo) return level;
            int col = Location.Col;
            int row = Location.Row;
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
                .WithPlayer(this with { AmmoCount = AmmoCount - 1 });
            return level;
        }
        public Player Take(ITakeable toTake) => this with { Inventory = Inventory.AddItem(toTake) };
        public bool Has<T>(ConsoleColor color) => Inventory.Where(item => item.MatchesColor<T>(color)).Any();

        /// <summary>
        /// If a player has an item in their inventory matching the specified type and color, return the first match of that item
        /// and a player with all matches removed from their inventory. Throws an exception if there are no matches.
        /// </summary>
        public (ITakeable item, Player newPlayerState) DropItem<T>(ConsoleColor color)
        { 
            ITakeable item = Inventory.Where(item => item.MatchesColor<T>(color)).First();
            Player newPlayerState = this with { Inventory = Inventory.Where(item => !item.MatchesColor<T>(color)).ToInventory() };
            return (item, newPlayerState);
        }
    }

    public static class InventoryExtensions
    {
        ///<summary>Returns true if an ITakeable is of the given type and color</summary> 
        public static bool MatchesColor<T>(this ITakeable item, ConsoleColor color) => item is T && item.Color == color;
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
        public IEnumerator<ITakeable> GetEnumerator() => takeables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => takeables.GetEnumerator();
        public override bool Equals([NotNullWhen(true)] object? obj)
         => obj != null && obj is Inventory other && this.ContainsSameItemsAs(other);
        public bool Equals(Inventory other) => this.ContainsSameItemsAs(other);
        private bool ContainsSameItemsAs(Inventory other)
            => takeables.IsSubsetOf(other) && other.takeables.IsSubsetOf(takeables);
        public static bool operator ==(Inventory left, Inventory right) => left.Equals(right);  
        public static bool operator !=(Inventory left, Inventory right) => !(left == right);
        public override int GetHashCode()
        {
            // Adapted from code by Jon Skeet:
            // https://stackoverflow.com/questions/8094867/good-gethashcode-override-for-list-of-foo-objects-respecting-the-order
            unchecked
            {
                return 19 + takeables.Select(takeable => 31 + takeable.GetHashCode()).Sum();
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
}
