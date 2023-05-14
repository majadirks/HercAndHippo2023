namespace HercAndHippoLibCs
{ 
    public record Wall(ConsoleColor Color, Location Location) : IDisplayable, ITouchable, IShootable
    {
        public string ConsoleDisplayString => "█";
        public ConsoleColor BackgroundColor => Color;
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.StopBullet(level,shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => Behaviors.NoReaction(level);
    }
    public record BreakableWall(ConsoleColor Color, Location Location) : IDisplayable, IShootable, ITouchable
    {
        public string ConsoleDisplayString => "▓";
        public ConsoleColor BackgroundColor => ConsoleColor.Black;
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.DieAndStopBullet(this, level, shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => Behaviors.NoReaction(level);    
    }
    public record Door(ConsoleColor BackgroundColor, Location Location) : IDisplayable, IShootable, ITouchable
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
}
