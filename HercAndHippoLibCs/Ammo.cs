namespace HercAndHippoLibCs;
public record Ammo(Location Location, AmmoCount Count) : HercAndHippoObj, ILocatable, ITouchable, IConsoleDisplayable
{
    public string ConsoleDisplayString => "ä";
    public Color Color => Color.Green;
    public Color BackgroundColor => Color.Black;

    public override bool BlocksMotion(Level level, ILocatable toBlock) => false;

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
        => touchedBy switch
        {
            Player p => level.Without(this).WithPlayer(p with
            {
                AmmoCount = level.Player.AmmoCount + Count
            }),
            _ => level.NoReaction()
        };

}
