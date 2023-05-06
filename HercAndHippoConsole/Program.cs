using HercAndHippoLibCs;
using System.Diagnostics;

const int REFRESH_RATE = 20;

Stopwatch sw = new();
Level curState = TestLevels.WallsLevel;
ConsoleKeyInfo keyInfo = default;
bool firstRender = true;

while (true)
{
    sw.Start();
    while (sw.ElapsedMilliseconds < REFRESH_RATE);
    
    Level newState = curState.RefreshCyclables(keyInfo);
    if (keyInfo.KeyChar == 'q') break;
    keyInfo = default;
    RefreshDisplay(curState, newState, firstRender);
    firstRender = false;
    curState = newState;
    ShowMessage("Press 'q' to quit...");
    if (Console.KeyAvailable) 
        keyInfo = Console.ReadKey();
    sw.Reset();
}

void RefreshDisplay(Level oldState, Level newState, bool forceRefresh)
{
    if (forceRefresh) Console.Clear();
    if (!forceRefresh && newState.HasSameStateAs(oldState)) return;
    ClearOld(oldState, newState);
    ShowNew(oldState, newState, forceRefresh);
}

void ShowNew(Level oldState, Level newState, bool forceRefresh)
{
    IEnumerable<IDisplayable> toDisplay = newState.LevelObjects
        .Where(obj => forceRefresh || !oldState.LevelObjects.Contains(obj))
        .OrderBy(d => d.Color).ThenBy(d => d.Location.Row).ThenBy(d => d.Location.Col);
    foreach (IDisplayable displayable in toDisplay)
    {
        WriteDisplayable(displayable);
    }
}

void ClearOld(Level oldState, Level newState)
{
    foreach (IDisplayable toRemove in oldState.LevelObjects.Where(obj => !newState.LevelObjects.Contains(obj)))
    {
        Console.SetCursorPosition(toRemove.Location.Col, toRemove.Location.Row);
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(' ');
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
