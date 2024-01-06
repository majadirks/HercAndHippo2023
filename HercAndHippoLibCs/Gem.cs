using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCs;

public record Gem(Color Color, Location Location, Health Health) : HercAndHippoObj, ILocatable, ITouchable, IConsoleDisplayable
{
    public Color BackgroundColor => Color.Black;

    public string ConsoleDisplayString => "◆";

    public override bool BlocksMotion(Level level, ILocatable toBlock) => false;

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        if (touchedBy is Player player)
        {
            Health newHealth = player.Health + this.Health;
            Player nextPlayer = player with { Health = newHealth };
            return level.WithPlayer(nextPlayer).Without(this);
        }
        else
            return Behaviors.NoReaction(level);
    }
}
