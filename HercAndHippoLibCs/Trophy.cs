using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCs;

public record Trophy(Location Location) : HercAndHippoObj, ILocatable, ITouchable, IConsoleDisplayable
{
    public Color Color => Color.Yellow;

    public Color BackgroundColor => Color.Black;

    public string ConsoleDisplayString => "Φ";

    public override bool BlocksMotion(Level level, ILocatable toBlock)
     => true;

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        if (touchedBy is not Player)
            return level.NoReaction();
        
        Hippo? hippo = level.Hippo;
        if (hippo == null)
            return level.Without(this).Lose().AddSecondaryObject(new Message("The hippo is dead! You lose!"));
        else if (hippo.LockedToPlayer)
            return level.Without(this).Win().AddSecondaryObject(new Message("You win!"));
        else
            return level.AddSecondaryObject(new Message("Bring the hippo safely to the trophy!"));
    }
}


