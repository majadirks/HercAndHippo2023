namespace HercAndHippoLibCs
{
    public record Door(Color BackgroundColor, Location Location) : HercAndHippoObj, ILocatable, IShootable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "◙";
        public Color Color => Color.Black;

        public bool StopsBullet => true;

        // Block all non-player object, or a player object without the relevant key
        public override bool BlocksMotion(Level level, ILocatable toBlock)
            => toBlock switch
            {
                Player p => !p.Has<Key>(BackgroundColor),
                _ => true
            };

        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => level.NoReaction();
        public Level OnTouch(Level level, Direction _, ITouchable touchedBy)
            => touchedBy switch
            {
                Player p => p.Has<Key>(BackgroundColor) ? TakeKeyAndDie(level, p) : level.NoReaction(),
                _ => level.NoReaction()
            };
        private Level TakeKeyAndDie(Level level, Player player)
        {
            (bool _, ITakeable? _, Player newPlayerState) = player.DropItem<Key>(BackgroundColor);
            Level newState = this.Die(level).WithPlayer(newPlayerState);
            return newState;
        }
    }
}
