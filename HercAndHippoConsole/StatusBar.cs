using HercAndHippoLibCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.PlayTo;

namespace HercAndHippoConsole
{
    public class StatusBar
    {
        private readonly int margin;
        public StatusBar(int margin)
        {
            this.margin = margin;
        }
        /*
         * - Status bar:
   Player: Health, ammo, inventory, Kinetic Energy, player location,
   Hippo: hippo location if any, hippo locked, 
   Level: gravity, cycle count
   Console: scroll status, buffer stats, estimated cycle time, use parallel, refresh types (percents)
         */
        public void ShowStatus(Level state)
        {
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Blue;

            Console.SetCursorPosition(0, Console.BufferHeight - margin);
            Player player = state.Player;
            Console.WriteLine($"Health: {player.Health}, Ammo: {player.AmmoCount}, Velocity: {player.Velocity}, Energy: {player.KineticEnergy}");
            string inventory = string.Join(", ", player.Inventory.Select(item => item.GetType().Name));
            Console.WriteLine($"Inventory: {inventory}");
            Console.WriteLine($"Display plan stats: {FutureStates.GetCacheStats()}");
            if (state.GetMessage() is Message message)
            {
                Console.WriteLine(message.Text);
            }
        }
    }
}
