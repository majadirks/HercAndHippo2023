
namespace HercAndHippoLibCs;

public record Message(string Text, int RemainingCycles) : HercAndHippoObj, ICyclable
{
    public override bool BlocksMotion(Level level, ILocatable toBlock) => false;

    public Level Cycle(Level level, ActionInput actionInput)
    {
        if (RemainingCycles <= 1)
            return level.Without(this);
        else
        {

            Message nextMessage = this with { RemainingCycles = RemainingCycles - 1};
            return level.Replace(this, nextMessage);
        }
    }

    public static readonly Message Ouch = new("Ouch!", 200);
}
