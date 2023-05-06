namespace HercAndHippoLibCs
{
   
    public record Wall(ConsoleColor Color, Location Location) : IDisplayable, ITouchable, IShootable
    {
        public string ConsoleDisplayString => "█";
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => OnShotBehaviors.StopBullet(level); // Cannot shoot through a wall
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => level; // Cannot pass through a wall
    }

    public record BreakableWall(ConsoleColor Color, Location Location) : IDisplayable, IShootable, ITouchable
    {
        public string ConsoleDisplayString => "▓";
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => OnShotBehaviors.DieAndStopBullet(this, level, shotBy);
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy) => level;    
    }
    public record Door(ConsoleColor Color, Location Location) : IDisplayable, IShootable //, ITouchable
    {
        public string ConsoleDisplayString => "D";
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => OnShotBehaviors.StopBullet(level); // Cannot shoot through a door

        //public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
        //    => touchedBy switch
        //        Player p => p.HasInventoryItem(
        //    }
        //{
        //    // ToDo: if touchedBy is not player, return level (like wall)
        //    // If touchedBy is player without matching key, act as wall
        //    // If touchedBy is player with key, remove key from player and die.
        //    throw new NotImplementedException();
    }
}
