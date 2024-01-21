
namespace HercAndHippoLibCs;

public record Message(string Text, int RemainingCycles = Message.DEFAULT_PERSISTANCE) : HercAndHippoObj, ICyclable
{
    public const int DEFAULT_PERSISTANCE = 200;
    public override bool BlocksMotion(Level level, ILocatable toBlock) => false;

    public Level Cycle(Level level, ActionInput actionInput)
    {
        if (RemainingCycles <= 1)
            return level.Without(this);
        else
        {

            Message nextMessage = this with { RemainingCycles = RemainingCycles - 1};
            return level.ReplaceIfPresent(this, nextMessage);
        }
    }

    public static readonly Message Ouch = new("Ouch!");
}
