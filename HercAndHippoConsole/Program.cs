using HercAndHippoLibCs;
using System.Linq;

Level level = TestLevels.WallsLevel;
ConsoleKeyInfo keyInfo = default;
while (true)
{
    keyInfo = Console.ReadKey();

    Console.Clear();
    // Update cyclable objects (eg move player, enemies, etc)
    IEnumerable<ICyclable> toCycle = level.LevelObjects.Where(disp => disp is ICyclable cylable).Select(c => (ICyclable)c);
    foreach (ICyclable c in toCycle) 
        level = c.Cycle(level, keyInfo);
    if (keyInfo.KeyChar == 'q') break;
    keyInfo = default;

    // Display objects in level
    IEnumerable<IDisplayable> toDisplay = level.LevelObjects.OrderBy(d => d.Color).ThenBy(d => d.Location.Row).ThenBy(d => d.Location.Col);
    foreach (IDisplayable displayable in toDisplay)
    {
        WriteDisplayable(displayable);
    }

    Console.SetCursorPosition(1, Console.BufferHeight - 2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Press 'q' to quit...");
    
}

void WriteDisplayable(IDisplayable displayable)
{
    Console.SetCursorPosition(displayable.Location.Col, displayable.Location.Row);
    Console.ForegroundColor = GetColor(displayable);
    Console.Write(DisplayString(displayable));
}

ConsoleColor GetColor(IDisplayable displayable)
=> displayable.Color switch
    {
        Color.Red => ConsoleColor.Red,
        Color.Yellow => ConsoleColor.Yellow,
        Color.Green => ConsoleColor.Green,
        Color.Blue => ConsoleColor.Blue,
        Color.Purple => ConsoleColor.Magenta,
        Color.Black => ConsoleColor.Black,
        Color.White => ConsoleColor.White,
        _ => ConsoleColor.White
    };

string DisplayString(IDisplayable displayable)
    => displayable switch
    {
        // Environment
        Wall _ => "█",
        BreakableWall _ => "▓",
        Door _ => "D",
        Player p => p.IsDead ? "RIP" : "☺",

        // Goodies
        Ammo _ => "ä",
        Bullet _ => "*",

        // Unknown
        _ => "?"
    };

