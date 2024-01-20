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
   Console: scroll status, buffer stats, estimated cycle time, use parallel, refresh types (percents)
         */
        public void ShowStatus(Level state)
        {          
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Blue;

            Console.SetCursorPosition(0, Console.BufferHeight - margin);
            Player player = state.Player;
            Hippo? hippo = state.Hippo;
            OverwriteLine($"{player.Health}, {player.AmmoCount}, {player.Velocity}, Energy: {player.KineticEnergy}");
            OverwriteLine(player.Inventory.ToString());
            string hippoStr = hippo == null ?
                "No hippo present." :
                hippo.LockedToPlayer ? $"Hippo locked, health = {hippo.Health}." : $"Hippo loose, health = {hippo.Health}.";
            string levelStr = $"Level: gravity {state.Gravity}, cycle count {state.Cycles}.";
            OverwriteLine(hippoStr + " " + levelStr);
            OverwriteLine($"Display plan stats: {FutureStates.GetCacheStats()}");
            if (state.GetMessage() is Message message)
                OverwriteLine(message.Text);
            else
                OverwriteLine(" ");
        }

        private void OverwriteLine(string str)
        {
            Console.WriteLine(str + new string(' ', Console.WindowWidth - str.Length));
        }
    }
}
