using System;
namespace HercAndHippoLibCs
{
    public interface ICyclable { Level Cycle(Level level, ConsoleKeyInfo keyInfo);  }
    public record Ammo(Location Location, AmmoCount Count) : IDisplayable, ITouchable
    {
        public ConsoleColor Color => ConsoleColor.Green;

        public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
        => level.Without(this)
                .WithPlayer(level.Player with 
                { 
                    Location = this.Location, 
                    AmmoCount = level.Player.AmmoCount + Count 
                });

        public string ConsoleDisplayString => "ä";


    }
}
