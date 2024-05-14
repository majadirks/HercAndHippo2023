using HercAndHippoLibCs;

namespace HercAndHippoConsole
{
    internal static class ColorExtensions
    {
        public static string ToHtmlColor(this Color color)
        {
            string result = color switch
            {
                Color.White => "white",
                Color.Black => "black",
                Color.Red => "#ff3300",
                Color.Yellow => "#ffff99",
                Color.Green => "#00ff00",
                Color.Cyan => "#00ffff",
                Color.Blue => "#0000ff",
                Color.Magenta => "#ff00ff",
                Color.DarkRed => "#800000", // Go Maroons!
                Color.DarkYellow => "#cc9900",
                Color.DarkGreen => "#006600",
                Color.DarkCyan => "#0066cc",
                Color.DarkBlue => "#000099",
                Color.DarkMagenta => "#660066",
                _ => "UNKNOWN"
                //_ => throw new NotSupportedException($"No known conversion from {color} to ConsoleColor.")
            };
            if (result == "UNKNOWN")
            {
                Console.WriteLine($"Unknown color {color}");
                return "#ffff99"; // yellow
            }
            else
            {
                return result;
            }
        }
    }
}
