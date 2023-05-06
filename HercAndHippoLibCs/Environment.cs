namespace HercAndHippoLibCs
{
   
    public record Wall(ConsoleColor Color, Location Location) : IDisplayable, ITouchable
    {
        public string ConsoleDisplayString => "█";
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => level;
    }

    public record BreakableWall(ConsoleColor Color, Location Location) : IDisplayable, IShootable, ITouchable
    {
        public string ConsoleDisplayString => "▓";
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => level.Without(this).Without(shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => level;    
    }
    public record Door(ConsoleColor Color, Location Location) : IDisplayable
    {
        // TODO: dies when correct key is used
        public string ConsoleDisplayString => "D";
    }; 
}