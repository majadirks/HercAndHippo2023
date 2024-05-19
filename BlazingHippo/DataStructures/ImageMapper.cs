using HercAndHippoConsole;
using HercAndHippoLibCs;
using Microsoft.AspNetCore.Components;
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
            Hippo => $@"<img src = "".\img\hippo.png"" style=""{phho.Location()}""/>",
            Player => $@"<img src = "".\img\herc.png"" style=""{phho.Location()}""/>",
            Groodle => $@"<img src = "".\img\groodle.png"" style=""{phho.Location()}""/>",
            Key => phho.Hho.Color == HercAndHippoLibCs.Color.DarkMagenta ?
                    $@"<img src = "".\img\purple_key.png"" style=""{phho.Location()}""/>" : "",
            _ => $@"<div style=""{phho.Location()} {phho.Color()}"">{phho.Hho.ConsoleDisplayString}</div>",
        };
        return new RenderFragment(b => b.AddMarkupContent(0, html));
    }
    private static string Location(this PlannedHho phho)
    => $"position:absolute; left:{phho.Col * WIDTH}px; top:{phho.Row * HEIGHT}px; width:{WIDTH}px; height:{HEIGHT}px;";

    private static string Color(this PlannedHho hho)
    => $"color:{hho.Hho.Color.ToHtmlColor()}; background-color:{hho.Hho.BackgroundColor.ToHtmlColor()};";

}
