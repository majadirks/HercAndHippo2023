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
            Message msg = new($"Gems give you health! Health: {nextPlayer.Health}");
            return level.WithPlayer(nextPlayer).AddSecondaryObject(msg).Without(this);
        }
        else
            return level.NoReaction();
    }
}
