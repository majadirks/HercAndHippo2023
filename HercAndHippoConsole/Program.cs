using HercAndHippoLibCs;

Level level = TestLevels.WallsLevel;

// Display objects in level
foreach (IDisplayable displayable in level.Displayables)
{
    Console.SetCursorPosition(displayable.Location.Col, displayable.Location.Row);
    Console.WriteLine("X");
}

// Display player
Console.SetCursorPosition(level.PlayerStart.Col, level.PlayerStart.Row);
Console.WriteLine("☺");

Console.SetCursorPosition(1, level.MaxRow + 3);
Console.WriteLine("Press Any Key to continue...");
Console.ReadKey();