namespace HercAndHippoLibCs
{
    public record Door(Color BackgroundColor, Location Location) : HercAndHippoObj, ILocatable, IShootable, ITouchable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "◙";
        public Color Color => Color.Black;

        public bool StopsBullet => true;

        public override bool BlocksMotion(Level level) => !level.Player.Has<Key>(BackgroundColor);

        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.NoReaction(level);
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
}
