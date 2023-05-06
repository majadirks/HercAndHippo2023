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

    public record Key(Location Location, ConsoleColor Color) : IDisplayable, ITouchable, ITakeable, IShootable
    {
        public string ConsoleDisplayString => "&";
        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
        {
            Player player = level.Player;
            Level stateWherePlayerHasKeyInInventory = player.Take(level, this).WithPlayer(player with { Location = this.Location });
            Level reactToKeyBeingTakenState = OnTake(stateWherePlayerHasKeyInInventory);
            return reactToKeyBeingTakenState;
        }
        public Level OnTake(Level level) => level.Without(this); // Die after being taken
        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => OnShotBehaviors.AllowBulletToPass(this, level, shotBy);
    }

}
