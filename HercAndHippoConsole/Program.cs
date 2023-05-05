using HercAndHippoLibCs;

Level level = TestLevels.WallsLevel;
ConsoleKeyInfo keyInfo = default;



while (keyInfo == default || keyInfo.KeyChar != 'q')
{
    Console.Clear();
    Player player = level.FindPlayer();
    // Display objects in level
    foreach (IDisplayable displayable in level.Displayables.OrderBy(d => d.Color).ThenBy(d => d.Location.Row).ThenBy(d => d.Location.Col))
    {
        Console.SetCursorPosition(displayable.Location.Col, displayable.Location.Row);
        Console.ForegroundColor = (displayable.Color switch
        {
            Color.Red => ConsoleColor.Red,
            Color.Yellow => ConsoleColor.Yellow,
            Color.Green => ConsoleColor.Green,
            Color.Blue => ConsoleColor.Blue,
            Color.Purple => ConsoleColor.Magenta,
            Color.Black => ConsoleColor.Black,
            Color.White => ConsoleColor.White,
            _ => ConsoleColor.White
        }) ;
        Console.WriteLine(displayable switch
        {
            Wall _ => "█",
            Player _ => "☺",
            _ => "?"
        });
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

