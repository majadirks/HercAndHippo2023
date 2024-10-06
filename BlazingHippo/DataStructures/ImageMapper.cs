using HercAndHippoConsole;
using HercAndHippoLibCs;
using Microsoft.AspNetCore.Components;
using static BlazingHippo.Constants;
namespace BlazingHippo;

public static class ImageMapper
{

    public static IEnumerable<IConsoleDisplayable> GetDisplayables(this Level level)
        => level.LevelObjects.Where(obj => obj is IConsoleDisplayable).Cast<IConsoleDisplayable>();
    public static RenderFragment GetHtml(this PlannedHho phho)
    {
        if (phho == null || phho.Hho == null)
            throw new Exception("Unexpected null");
        Color color = phho.Hho.Color;
        Color bg = phho.Hho.BackgroundColor;
        string html = phho.Hho switch
        {
            Hippo => img("hippo.png"),
            Player => img("herc.png"),
            Groodle => img("groodle.png"),
            Key =>
                color == Color.DarkMagenta ? img("purple_key.png") :
                NoImageFile(),
            BreakableWall => img("lattice.png"),
            Wall => img("stone_wall.png"),
            Door =>
                bg == Color.DarkMagenta ? img("purple_door.png") : NoImageFile(),
            Ammo => img("quiver.png"),
            Gem =>
                color == Color.Red ? img("red_gem.png") : NoImageFile(),
            Trophy => img("trophy.png"),
            Bullet b => 
                b.Whither == Direction.East ? img("arrow_right.png") :
                b.Whither == Direction.North ? img("arrow_up.png") :
                b.Whither == Direction.South ? img("arrow_down.png") :
                b.Whither == Direction.West ? img("arrow_left.png") :
                NoImageFile(),
            BounceBuddy => img("bouncebuddy.png"),
            _ => NoImageFile(),

        }; 
        return new RenderFragment(b => b.AddMarkupContent(0, html));

        string img(string file)
            => $@"<img src = "".\img\{file}"" style=""{phho.LocationStyle()}""/>";
        string NoImageFile()
        => $@"<div style=""{phho.LocationStyle()} {phho.ColorStyle()}"">{phho.Hho.ConsoleDisplayString}</div>";
    }
    private static string LocationStyle(this PlannedHho phho)
    => $"position:absolute; left:{phho.Col * IMG_WIDTH}px; top:{phho.Row * IMG_HEIGHT}px; width:{IMG_WIDTH}px; height:{IMG_HEIGHT}px;";

    private static string ColorStyle(this PlannedHho hho)
    => $"color:{hho.Hho.Color.ToHtmlColor()}; background-color:{hho.Hho.BackgroundColor.ToHtmlColor()};";

}
