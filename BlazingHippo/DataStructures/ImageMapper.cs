using HercAndHippoConsole;
using HercAndHippoLibCs;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
namespace BlazingHippo;

public static class ImageMapper
{
    public const int WIDTH = 20;
    public const int HEIGHT = 32;
    public static IEnumerable<IConsoleDisplayable> GetDisplayables(this Level level)
        => level.LevelObjects.Where(obj => obj is IConsoleDisplayable).Cast<IConsoleDisplayable>();
    public static RenderFragment GetHtml(this PlannedHho phho)
    {
        if (phho == null || phho.Hho == null)
            throw new Exception("Unexpected null");

        string html = phho.Hho switch
        {
            Hippo => img("hippo.png"),
            Player => img("herc.png"),
            Groodle => img("groodle.png"),
            Key => 
                phho.Hho.Color == HercAndHippoLibCs.Color.DarkMagenta ? img("purple_key.png") : 
                NoImageFile(),
            BreakableWall => img("lattice.png"),
            Wall => img("stone_wall.png"),
            _ => NoImageFile(),
        };
        return new RenderFragment(b => b.AddMarkupContent(0, html));

        string img(string file)
            => $@"<img src = "".\img\{file}"" style=""{phho.Location()}""/>";
        string NoImageFile()
        => $@"<div style=""{phho.Location()} {phho.Color()}"">{phho.Hho.ConsoleDisplayString}</div>";
    }
    private static string Location(this PlannedHho phho)
    => $"position:absolute; left:{phho.Col * WIDTH}px; top:{phho.Row * HEIGHT}px; width:{WIDTH}px; height:{HEIGHT}px;";

    private static string Color(this PlannedHho hho)
    => $"color:{hho.Hho.Color.ToHtmlColor()}; background-color:{hho.Hho.BackgroundColor.ToHtmlColor()};";

}
