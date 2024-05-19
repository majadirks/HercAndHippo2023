using HercAndHippoConsole;
using HercAndHippoLibCs;
using Microsoft.AspNetCore.Components;
namespace BlazingHippo;

public static class ImageMapper
{
    public const int WIDTH = 16;
    public const int HEIGHT = 32;
    public static IEnumerable<IConsoleDisplayable> GetDisplayables(this Level level)
        => level.LevelObjects.Where(obj => obj is IConsoleDisplayable).Cast<IConsoleDisplayable>();
    //public static RenderFragment GetHtml(this HercAndHippoObj hho)
    //{
    //    if (hho is not IConsoleDisplayable dhho)
    //        return new RenderFragment(b => b.AddMarkupContent(0, ""));

    //    int id = hho.Id;
    //    string html = hho switch
    //    {
    //        Hippo => $@"<img id=""{id}"" src = "".\img\hippo.jpeg"" style=""{dhho.Location()}""/>",
    //        Player => $@"<img id=""{id}"" src = "".\img\herc.jpeg"" style=""{dhho.Location()}""/>",
    //        Groodle => $@"<img id=""{id}"" src = "".\img\groodle.png"" style=""{dhho.Location()}""/>",
    //        _ => $@"<div id=""{id}"" style=""{dhho.Location()} {dhho.Color()}"">{dhho.ConsoleDisplayString}</div>",
    //    };
    //    return new RenderFragment(b => b.AddMarkupContent(0, html));
    //}

    public static RenderFragment GetHtml(this PlannedHho phho)
    {
        if (phho == null || phho.Hho == null)
            throw new Exception("Unexpected null");

        string html = phho.Hho switch
        {
            Hippo => $@"<img src = "".\img\hippo.jpeg"" style=""{phho.Location()}""/>",
            Player => $@"<img src = "".\img\herc.jpeg"" style=""{phho.Location()}""/>",
            Groodle => $@"<img src = "".\img\groodle.png"" style=""{phho.Location()}""/>",
            _ => $@"<div style=""{phho.Location()} {phho.Color()}"">{phho.Hho.ConsoleDisplayString}</div>",
            //_ => $@"<div style=""{phho.Location()} {phho.Color()}"">&</div>",
        };
        return new RenderFragment(b => b.AddMarkupContent(0, html));
    }

    //private static string Location(this IConsoleDisplayable hho)
    //    => $"position:absolute; left:{hho.Location.Col * WIDTH}px; top:{hho.Location.Row * HEIGHT}px; width:{WIDTH}px; height:{HEIGHT}px;";

    private static string Location(this PlannedHho phho)
    => $"position:absolute; left:{phho.Col * WIDTH}px; top:{phho.Row * HEIGHT}px; width:{WIDTH}px; height:{HEIGHT}px;";


    //private static string Color(this IConsoleDisplayable hho) 
    //    => $"color:{hho.Color.ToHtmlColor()}; background-color:{hho.BackgroundColor.ToHtmlColor()};";

    private static string Color(this PlannedHho hho)
    => $"color:{hho.Hho.Color.ToHtmlColor()}; background-color:{hho.Hho.BackgroundColor.ToHtmlColor()};";

}
