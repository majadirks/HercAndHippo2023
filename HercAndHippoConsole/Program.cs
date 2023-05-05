﻿using HercAndHippoLibCs;
using System.Linq;

Level level = TestLevels.WallsLevel;
ConsoleKeyInfo keyInfo = default;
while (true)
{
    level = level.RefreshCyclables(keyInfo);
    if (keyInfo.KeyChar == 'q') break;
    keyInfo = default;
    // Display objects in level
    RefreshDisplay(level, level);
    ShowMessage("Press 'q' to quit...");
    keyInfo = Console.ReadKey();
}

void RefreshDisplay(Level oldState, Level newState)
{
    // ToDo : logic to check if refresh is needed
    Console.Clear();
    IEnumerable<IDisplayable> toDisplay = newState.LevelObjects.OrderBy(d => d.Color).ThenBy(d => d.Location.Row).ThenBy(d => d.Location.Col);
    foreach (IDisplayable displayable in toDisplay)
    {
        WriteDisplayable(displayable);
    }
}

void ShowMessage(string message)
{
    Console.SetCursorPosition(1, Console.BufferHeight - 2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(message);
}

void WriteDisplayable(IDisplayable displayable)
{
    Console.SetCursorPosition(displayable.Location.Col, displayable.Location.Row);
    Console.ForegroundColor = GetColor(displayable);
    Console.Write(displayable.ConsoleDisplayString);
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
