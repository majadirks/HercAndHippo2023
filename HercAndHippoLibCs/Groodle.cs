using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCs;

public record Groodle(Location Location, Direction Whither) : HercAndHippoObj, IShootable, ITouchable, ILocatable, IConsoleDisplayable, ICyclable
{
    public bool StopsBullet => true;

    public Color Color => Color.White;

    public Color BackgroundColor => Color.Black;

    public string ConsoleDisplayString => "τ";

    public override bool BlocksMotion(Level level) => false;

    public Level Cycle(Level level, ActionInput actionInput)
    {
        Level nextLevel = level;
        Groodle nextGroodle = this;
        if (Location == nextLevel.Player.Location)
            nextLevel = OnTouch(level, Direction.Idle, nextLevel.Player);
        if (Location == nextLevel.Hippo?.Location)
            nextLevel = OnTouch(level, Direction.Idle, nextLevel.Hippo);

        if (Whither == Direction.East)
        {
            nextGroodle =
                this.MotionBlockedTo(nextLevel, Direction.East) ?
                this with { Whither = Direction.West } :
                this with { Location = new(Location.Col.NextEast(nextLevel.Width), Location.Row) };
        }
        else if (Whither == Direction.West)
        {
            nextGroodle =
                this.MotionBlockedTo(nextLevel, Direction.West) ?
                this with { Whither = Direction.East } :
                this with { Location = new(Location.Col.NextWest(), Location.Row) };
        }
        
        // ToDo: gravity
        // ToDo: interact with hippo if blocked
        return nextLevel.Replace(this, nextGroodle);
    }

    public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.Die(level, this);

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        if (touchedBy is Player player)
        {
            Health nextHealth = player.Health - 5;
            Player nextPlayer = player with { Health = nextHealth };
            return level.WithPlayer(nextPlayer);
        }
        else if (touchedBy is Hippo hippo)
        {
            Health nextHealth = hippo.Health - 5;
            Hippo nextHippo = hippo with { Health = nextHealth };
            return level.Replace(hippo, nextHippo);
        }
        else
            return Behaviors.NoReaction(level);
    }
}
