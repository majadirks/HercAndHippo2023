namespace HercAndHippoLibCs;
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
