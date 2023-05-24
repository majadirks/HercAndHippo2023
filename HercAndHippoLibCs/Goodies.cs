namespace HercAndHippoLibCs
{
    public record Ammo(Location Location, AmmoCount Count) : HercAndHippoObj, ILocatable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "ä";

        public ConsoleColor Color => ConsoleColor.Green;
        public ConsoleColor BackgroundColor => ConsoleColor.Black;
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
            => level.Without(this)
                    .WithPlayer(level.Player with 
                                { 
                                    Location = this.Location, 
                                    AmmoCount = level.Player.AmmoCount + Count 
                                });
    }

    public record Key(ConsoleColor Color, Location Location) : HercAndHippoObj, ILocatable, ITouchable, ITakeable
    {
        public string ConsoleDisplayString => "♀";
        public ConsoleColor BackgroundColor => ConsoleColor.Black;
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
        {
            Player player = level.Player;
            Level stateWherePlayerHasKeyInInventory = level.WithPlayer(player.Take(this) with { Location = this.Location });
            Level reactToKeyBeingTakenState = OnTake(stateWherePlayerHasKeyInInventory);
            return reactToKeyBeingTakenState;
        }
        public Level OnTake(Level level) => level.Without(this); // Die after being taken
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.AllowBulletToPass(this, level, shotBy);
    }

}
