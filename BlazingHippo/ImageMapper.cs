using HercAndHippoLibCs;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace BlazingHippo
{
    public static class ImageMapper
    {
        public const int WIDTH = 16;
        public const int HEIGHT = 32;
        public static RenderFragment GetHtml(this IConsoleDisplayable hho)
        {
            string html = hho switch
            {
                Hippo => $@"<img src = "".\img\hippo.jpeg"" style=""{hho.Location()}""/>",
                Player => $@"<img src = "".\img\herc.jpeg"" style=""{hho.Location()}""/>",
                Groodle => $@"<img src = "".\img\groodle.png"" style=""{hho.Location()}""/>",
                _ => $@"<div style=""{hho.Location()} {hho.Color()}"">{hho.ConsoleDisplayString}</div>",
            };
            return new RenderFragment(b => b.AddMarkupContent(0, html));
        }

        private static string Location(this IConsoleDisplayable hho)
            => $"position:absolute; left:{hho.Location.Col * WIDTH}px; top:{hho.Location.Row * HEIGHT}px; width:{WIDTH}px; height:{HEIGHT}px;";

        private static string Color(this IConsoleDisplayable hho) 
            => $"color:{hho.Color}; background-color:{hho.BackgroundColor};";

    }
}
