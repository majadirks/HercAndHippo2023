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
                phho.Hho.Color == Color.DarkMagenta ? img("purple_key.png") : 
                NoImageFile(),
            BreakableWall => img("lattice.png"),
            Wall => img("stone_wall.png"),
            Door => img("purple_door.png"),
                //phho.Hho.Color == Color.DarkMagenta ? img("purple_door.png") :
                //NoImageFile(),
            _ => NoImageFile(),
        };
        return new RenderFragment(b => b.AddMarkupContent(0, html));

        string img(string file)
            => $@"<img src = "".\img\{file}"" style=""{phho.LocationStyle()}""/>";
        string NoImageFile()
        => $@"<div style=""{phho.LocationStyle()} {phho.ColorStyle()}"">{phho.Hho.ConsoleDisplayString}</div>";
    }
    private static string LocationStyle(this PlannedHho phho)
    => $"position:absolute; left:{phho.Col * WIDTH}px; top:{phho.Row * HEIGHT}px; width:{WIDTH}px; height:{HEIGHT}px;";

    private static string ColorStyle(this PlannedHho hho)
    => $"color:{hho.Hho.Color.ToHtmlColor()}; background-color:{hho.Hho.BackgroundColor.ToHtmlColor()};";

}
