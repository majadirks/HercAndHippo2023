using HercAndHippoLibCs;

Level level = TestLevels.WallsLevel;
ConsoleKeyInfo keyInfo = default;



while (keyInfo == default || keyInfo.KeyChar != 'q')
{
    Console.Clear();
    Player player = level.FindPlayer();
    // Display objects in level
    foreach (IDisplayable displayable in level.Displayables.OrderBy(d => d.Location.Row).ThenBy(d => d.Location.Col))
    {
        Console.SetCursorPosition(displayable.Location.Col, displayable.Location.Row);
        Console.WriteLine(displayable switch
        {
            Wall _ => "X",
            Player _ => "☺",
            _ => "?"
        });
    }
    Console.SetCursorPosition(1, Math.Min(level.MaxRow + 3, Console.BufferHeight));
    Console.WriteLine("Press 'q' to quit...");
    keyInfo = Console.ReadKey();
    if (keyInfo.Key == ConsoleKey.LeftArrow) level = level.WithPlayer(player.MoveLeft());
    if (keyInfo.Key == ConsoleKey.RightArrow) level = level.WithPlayer(player.MoveRight());
    if (keyInfo.Key == ConsoleKey.UpArrow) level = level.WithPlayer(player.MoveUp());
    if (keyInfo.Key == ConsoleKey.DownArrow) level = level.WithPlayer(player.MoveDown());
}

