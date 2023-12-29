namespace HercAndHippoLibCs
{
    public record Ammo(Location Location, AmmoCount Count) : HercAndHippoObj, ILocatable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "ä";
        public Color Color => Color.Green;
        public Color BackgroundColor => Color.Black;

        public override bool BlocksMotion(Player p) => false;

        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
            => level.Without(this)
                    .WithPlayer(level.Player with 
                                { 
                                    AmmoCount = level.Player.AmmoCount + Count 
                                });
    }

    public record Key(Color Color, Location Location) : HercAndHippoObj, ILocatable, IConsoleDisplayable, ITouchable, ITakeable
    {
        public string ConsoleDisplayString => "♀";
        public Color BackgroundColor => Color.Black;

        public string Id => Color.ToString();

        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
        {
            Player player = level.Player;
            Level stateWherePlayerHasKeyInInventory = level.WithPlayer(player.Take(this));
            Level reactToKeyBeingTakenState = OnTake(stateWherePlayerHasKeyInInventory);
            return reactToKeyBeingTakenState;
        }
        public Level OnTake(Level level) => level.Without(this); // Die after being taken
        public Level OnShot(Level level, Direction _, Bullet shotBy) => Behaviors.AllowBulletToPass(this, level, shotBy);

        public override bool BlocksMotion(Player p) => false;
    }

}
