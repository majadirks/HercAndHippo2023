using System.Globalization;

namespace HercAndHippoLibCs
{ 
    public record Wall(ConsoleColor Color, Location Location) : HercAndHippoObj, ILocatable, ITouchable, IShootable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "█";
        public ConsoleColor BackgroundColor => Color;
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.StopBullet(level,shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => Behaviors.NoReaction(level);
    }
    public record BreakableWall(ConsoleColor Color, Location Location) : HercAndHippoObj, ILocatable, IShootable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "▓";
        public ConsoleColor BackgroundColor => ConsoleColor.Black;
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.DieAndStopBullet(this, level, shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => Behaviors.NoReaction(level);    
    }
    public record Door(ConsoleColor BackgroundColor, Location Location) : HercAndHippoObj, ILocatable, IShootable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "◙";
        public ConsoleColor Color => ConsoleColor.Black;
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.StopBullet(level,shotBy); // Cannot shoot through a door
        public Level OnTouch(Level level, Direction _, ITouchable touchedBy)
            => touchedBy switch
            {
                Player p => p.Has<Key>(BackgroundColor) ? TakeKeyDieAndAllowPassage(level, p) : Behaviors.NoReaction(level),
                _ => Behaviors.NoReaction(level)
            };
        private Level TakeKeyDieAndAllowPassage(Level level, Player player)
        {
            (ITakeable _, Player newPlayerState) = player.DropItem<Key>(BackgroundColor);
            Level newState = Behaviors.DieAndAllowPassage(level, this, newPlayerState);
            return newState;
        }
    }

    public record Driver(Direction Whither, int Modulus, int Count = 0) : HercAndHippoObj, ICyclable
    {
        public Level Cycle(Level level, ActionInput actionInput)
        {
            int nextCount = Count + 1;
            int nextMod = Modulus;

            if (actionInput == ActionInput.MoveNorth) nextMod = Math.Max(1, nextMod - 1);
            else if (actionInput == ActionInput.MoveSouth) nextMod = Math.Min(10, nextMod + 1);

            if (Count % Modulus > 0)
                return level.Replace(this, this with { Count = nextCount, Modulus = nextMod} );

            ActionInput motion = Whither switch
            {
                Direction.East => ActionInput.MoveEast,
                Direction.West => ActionInput.MoveWest,
                Direction.North => ActionInput.MoveNorth,
                Direction.South => ActionInput.MoveSouth,
                _ => ActionInput.NoAction

            };
            return level.Player.Cycle(level, motion).Replace(this, this with { Count = nextCount, Modulus = nextMod });
        }
    }
}
