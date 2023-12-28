using System.Globalization;

namespace HercAndHippoLibCs
{ 
    public record Wall(Color Color, Location Location) : HercAndHippoObj, ILocatable, ITouchable, IShootable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "█";
        public Color BackgroundColor => Color;

        public override bool BlocksMotion(Player p) => true;

        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.StopBullet(level,shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => Behaviors.NoReaction(level);
    }
    public record BreakableWall(Color Color, Location Location) : HercAndHippoObj, ILocatable, IShootable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "▓";
        public Color BackgroundColor => Color.Black;

        public override bool BlocksMotion(Player p) => true;

        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.DieAndStopBullet(this, level, shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => Behaviors.NoReaction(level);    
    }
    public record Door(Color BackgroundColor, Location Location) : HercAndHippoObj, ILocatable, IShootable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "◙";
        public Color Color => Color.Black;

        public override bool BlocksMotion(Player p) => !p.Has<Key>(BackgroundColor);

        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.StopBullet(level,shotBy); // Cannot shoot through a door
        public Level OnTouch(Level level, Direction _, ITouchable touchedBy)
            => touchedBy switch
            {
                Player p => p.Has<Key>(BackgroundColor) ? TakeKeyAndDie(level, p) : Behaviors.NoReaction(level),
                _ => Behaviors.NoReaction(level)
            };
        private Level TakeKeyAndDie(Level level, Player player)
        {
            (bool _, ITakeable? _, Player newPlayerState) = player.DropItem<Key>(BackgroundColor);
            Level newState = Behaviors.Die(level, this).WithPlayer(newPlayerState);
            return newState;
        }
    }

    public record Driver(Direction Whither, int Modulus, int Count = 0) : HercAndHippoObj, ICyclable
    {
        public override bool BlocksMotion(Player p) => false;

        public Level Cycle(Level level, ActionInput actionInput)
        {
            int nextCount = Count + 1;
            int nextMod = Modulus;
            Level nextState = level;

            if (actionInput == ActionInput.MoveNorth)
            {
                nextMod = Math.Max(1, nextMod - 1);
                nextState = nextState.Player.Cycle(nextState, ActionInput.MoveSouth);
            }
            else if (actionInput == ActionInput.MoveSouth)
            {
                nextMod = Math.Min(10, nextMod + 1);
                nextState = nextState.Player.Cycle(nextState, ActionInput.MoveNorth);
            }

            if (Count % Modulus > 0)
                nextState = nextState.Replace(this, this with { Count = nextCount, Modulus = nextMod} );

            ActionInput motion = Whither switch
            {
                Direction.East => ActionInput.MoveEast,
                Direction.West => ActionInput.MoveWest,
                Direction.North => ActionInput.MoveNorth,
                Direction.South => ActionInput.MoveSouth,
                _ => ActionInput.NoAction

            };

            return nextState.Player.Cycle(level, motion).Replace(this, this with { Count = nextCount, Modulus = nextMod });
        }
    }
}
