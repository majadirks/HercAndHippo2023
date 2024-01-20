namespace HercAndHippoLibCs;
public record Key(Color Color, Location Location) : HercAndHippoObj, ILocatable, IConsoleDisplayable, ITouchable, ITakeable
{
    public string ConsoleDisplayString => "♀";
    public Color BackgroundColor => Color.Black;

    public string Id => Color.ToString();

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        if (touchedBy is Player player)
        {
            Level stateWherePlayerHasKeyInInventory = level.WithPlayer(player.Take(this));
            Level reactToKeyBeingTakenState = OnTake(stateWherePlayerHasKeyInInventory);
            return reactToKeyBeingTakenState;
        }
        else
            return level.NoReaction();
    }
    public Level OnTake(Level level) => level.Without(this); // Die after being taken
    public override bool BlocksMotion(Level level, ILocatable toBlock) => false;

    public override string ToString() => $"{Color} Key";
}
