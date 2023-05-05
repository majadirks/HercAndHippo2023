using HercAndHippoLibCs;

Level level = TestLevels.WallsLevel;
ConsoleKeyInfo keyInfo = default;

while (keyInfo == default || keyInfo.KeyChar != 'q')
{
    Console.Clear();
    Player player = level.FindPlayer();
    // Display objects in level
    IEnumerable<IDisplayable> toDisplay = level.Displayables.OrderBy(d => d.Color).ThenBy(d => d.Location.Row).ThenBy(d => d.Location.Col);
    foreach (IDisplayable displayable in toDisplay)
    {
       WriteDisplayable(displayable);
    }

    Console.SetCursorPosition(1, Console.BufferHeight - 2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Press 'q' to quit...");
    keyInfo = Console.ReadKey();
    if (keyInfo.Key == ConsoleKey.LeftArrow) level = level.WithPlayer(player.MoveLeft());
    if (keyInfo.Key == ConsoleKey.RightArrow) level = level.WithPlayer(player.MoveRight());
    if (keyInfo.Key == ConsoleKey.UpArrow) level = level.WithPlayer(player.MoveUp());
    if (keyInfo.Key == ConsoleKey.DownArrow) level = level.WithPlayer(player.MoveDown());
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
        Wall _ => "█",
        BreakableWall _ => "▓",
        Door _ => "D",
        Player _ => "☺",
        _ => "?"
    };

