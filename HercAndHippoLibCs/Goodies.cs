using System;
namespace HercAndHippoLibCs
{
    public record Ammo(Location Location, AmmoCount Count) : IDisplayable, ITouchable
    {
        public Color Color => Color.Green;

        public Level OnTouch(Level level, Direction touchedFrom)
        {
            Player player = level.FindPlayer();
            return level
                .Without(this)
                .WithPlayer(player with 
                { 
                    Location= this.Location, 
                    Ammo = player.Ammo + Count 
                });
        }
    }
}
