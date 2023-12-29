namespace HercAndHippoLibCs;
public record Ammo(Location Location, AmmoCount Count) : HercAndHippoObj, ILocatable, ITouchable, IConsoleDisplayable
{
    public string ConsoleDisplayString => "ä";
    public Color Color => Color.Green;
    public Color BackgroundColor => Color.Black;

    public override bool BlocksMotion(Level level) => false;

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
        => level.Without(this)
                .WithPlayer(level.Player with 
                            { 
                                AmmoCount = level.Player.AmmoCount + Count 
                            });
}
