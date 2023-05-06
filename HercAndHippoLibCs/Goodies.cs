namespace HercAndHippoLibCs
{
    public record Ammo(Location Location, AmmoCount Count) : IDisplayable, ITouchable
    {
        public string ConsoleDisplayString => "ä";
        public ConsoleColor Color => ConsoleColor.Green;
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
            => level.Without(this)
                    .WithPlayer(level.Player with 
                                { 
                                    Location = this.Location, 
                                    AmmoCount = level.Player.AmmoCount + Count 
                                });
        
    }
}
