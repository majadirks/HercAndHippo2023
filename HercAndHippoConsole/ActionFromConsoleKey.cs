using HercAndHippoLibCs;

namespace HercAndHippoConsole
{
    public static class ActionFromConsoleKey
    {
        public static ActionInput ToActionInput(this System.ConsoleKeyInfo keyInfo)
        {
            if (keyInfo == default) return ActionInput.NoAction;
            // Shift key pressed (shoot)
            if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
            {
                return keyInfo.Key switch
                {
                    ConsoleKey.LeftArrow => ActionInput.ShootWest,
                    ConsoleKey.RightArrow => ActionInput.ShootEast,
                    ConsoleKey.UpArrow => ActionInput.ShootNorth,
                    ConsoleKey.DownArrow => ActionInput.ShootSouth,
                    _ => ActionInput.NoAction  // No update for unknown key
                };
            }
            // No shift key; move player
            return keyInfo.Key switch
            {
                ConsoleKey.LeftArrow => ActionInput.MoveWest,
                ConsoleKey.RightArrow => ActionInput.MoveEast,
                ConsoleKey.UpArrow => ActionInput.MoveNorth,
                ConsoleKey.DownArrow => ActionInput.MoveSouth,
                _ => ActionInput.NoAction // No update for unknown key
            };
        }
    }
}
