namespace HercAndHippoLibCs
{
    public record Player(Location Location, Health Health, AmmoCount AmmoCount) : IDisplayable, IShootable, ICyclable, ITouchable
    {
        public string ConsoleDisplayString => HasHealth ? "☺" : "RIP";
        public ConsoleColor Color => ConsoleColor.Blue;
        public override string ToString() => $"Player at location {Location} with {Health}, {AmmoCount}";
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy)
            => level.WithPlayer(this with { Health = Health - 5 });
        public bool HasHealth => Health.HasHealth;
        public bool HasAmmo => AmmoCount.HasAmmo;
        public Level Cycle(Level level, ConsoleKeyInfo keyInfo)
        {
            // Shift key pressed (shoot)
            if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
            {
                return keyInfo.Key switch
                {
                    ConsoleKey.LeftArrow => Shoot(level, Direction.West),
                    ConsoleKey.RightArrow => Shoot(level, Direction.East),
                    ConsoleKey.UpArrow => Shoot(level, Direction.North),
                    ConsoleKey.DownArrow => Shoot(level, Direction.South),
                    _ => level // No update for unknown key
                };
            }
            // No shift key; move player
            return keyInfo.Key switch
            {
                ConsoleKey.LeftArrow => MoveLeft(level),
                ConsoleKey.RightArrow => MoveRight(level),
                ConsoleKey.UpArrow => MoveUp(level),
                ConsoleKey.DownArrow => MoveDown(level),
                _ => level // No update for unknown key
            };
        }
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
            => touchedBy switch
            {
                Bullet shotBy => OnShot(level, touchedFrom.Mirror(), (Bullet)shotBy),
                _ => level
            };
        private Level TryMoveTo(Location newLocation, Direction approachFrom, Level curState)
            => curState.ObjectAt(newLocation) switch
                {
                    ITouchable touchableAtLocation => touchableAtLocation.OnTouch(curState, approachFrom, this),
                    _ => curState.WithPlayer(this with { Location = newLocation })
                };
   
        private Level MoveLeft(Level level) => TryMoveTo((Location.Col - 1, Location.Row), approachFrom: Direction.East, curState: level);
        private Level MoveRight(Level level) => TryMoveTo((Location.Col + 1, Location.Row), Direction.West, curState: level);
        private Level MoveUp(Level level) => TryMoveTo((Location.Col, Location.Row - 1), Direction.South, curState: level);
        private Level MoveDown(Level level) => TryMoveTo((Location.Col, Location.Row + 1), Direction.North, curState: level);
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
    }
    public readonly struct Health
    {
        private const int MIN_HEALTH = 0;
        private const int MAX_HEALTH = 200;
        private const int DEFAULT_STARTING_HEALTH = 100;
        private readonly int HealthAmt { get; init; }
        public Health(int health = DEFAULT_STARTING_HEALTH)
        {
            if (!IsValid(health)) 
                throw new ArgumentException($"Invalid health value {health} (must be between {MIN_HEALTH} and {MAX_HEALTH}");
            HealthAmt = health;
        }
        public bool HasHealth => HealthAmt > 0;
        private static bool IsValid(int health) => MIN_HEALTH <= health && health <= MAX_HEALTH;
        public static Health operator -(Health health, int subtrahend) => Math.Max(MIN_HEALTH, health.HealthAmt - subtrahend);
        public static Health operator +(Health health,int addend) => Math.Min(MAX_HEALTH, health.HealthAmt + addend);

        public static implicit operator Health(int health) => new(health);
        public override string ToString() => $"Health: {HealthAmt}";
    }

    public readonly struct AmmoCount
    {
        private const int MIN_AMMO = 0;
        private const int DEFAULT_STARTING_AMMO = 0;
        private readonly int AmmoAmt { get; init; }
        public AmmoCount(int ammo = DEFAULT_STARTING_AMMO)
        {
            if (!IsValid(ammo))
                throw new ArgumentException($"Invalid ammo value {ammo}; must be nonnegative");
            AmmoAmt = ammo;
        }
        public bool HasAmmo => AmmoAmt > 0;
        private static bool IsValid(int ammo) => MIN_AMMO <= ammo;
        public static AmmoCount operator -(AmmoCount ammo, int subtrahend) => Math.Max(MIN_AMMO, ammo.AmmoAmt - subtrahend);
        public static AmmoCount operator +(AmmoCount ammo, int addend) => ammo.AmmoAmt + addend;

        public static implicit operator AmmoCount(int ammo) => new(ammo);
        public static implicit operator int(AmmoCount ac) => ac.AmmoAmt;
        public override string ToString() => $"Ammo Count: {AmmoAmt}";
    }
}
