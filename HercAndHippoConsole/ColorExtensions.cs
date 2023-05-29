using HercAndHippoLibCs;

namespace HercAndHippoConsole
{
    internal static class ColorExtensions
    {
        public static ConsoleColor ToConsoleColor(this Color color)
            => color switch
            {
                Color.White => ConsoleColor.White,
                Color.Black => ConsoleColor.Black,
                Color.Red => ConsoleColor.Red,
                Color.Yellow => ConsoleColor.Yellow,
                Color.Green => ConsoleColor.Green,
                Color.Cyan => ConsoleColor.Cyan,
                Color.Blue => ConsoleColor.Blue,
                Color.Magenta => ConsoleColor.Magenta,
                Color.DarkRed => ConsoleColor.DarkRed,
                Color.DarkYellow => ConsoleColor.DarkYellow,
                Color.DarkGreen => ConsoleColor.DarkGreen,
                Color.DarkCyan => ConsoleColor.DarkCyan,
                Color.DarkBlue => ConsoleColor.DarkBlue,
                Color.DarkMagenta => ConsoleColor.DarkMagenta,
                _ => throw new NotSupportedException($"No known conversion from {color} to ConsoleColor.")
            };
    }
}
