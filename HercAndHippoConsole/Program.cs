using HercAndHippoLibCs;
using System.Diagnostics;

const int REFRESH_RATE = 20;

Stopwatch sw = new();
Level curState = TestLevels.WallsLevel;
ConsoleKeyInfo keyInfo = default;
bool firstRender = true;
sw.Start();

while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_RATE);
    sw.Restart();

    Level newState = curState.RefreshCyclables(keyInfo);
    if (keyInfo.KeyChar == 'q') break;
    keyInfo = default;
    RefreshDisplay(curState, newState, firstRender);
    firstRender = false;
    curState = newState;
    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
    if (Console.KeyAvailable) 
        keyInfo = Console.ReadKey();  
}

void RefreshDisplay(Level oldState, Level newState, bool forceRefresh)
{
    if (forceRefresh) Console.Clear();
    if (!forceRefresh && newState.HasSameStateAs(oldState)) return;
    ClearOld(oldState, newState);
    ShowNew(oldState, newState, forceRefresh);
}

void ShowNew(Level oldState, Level newState, bool forceRefresh) // ToDo: Need to re-show things that were previously overlapped (eg bullet passing over key)
{
    IEnumerable<IDisplayable> toDisplay = newState.LevelObjects
        .Where(obj => forceRefresh || !oldState.LevelObjects.Contains(obj) || oldState.ObjectsAt(obj.Location).Count() > 1)
        .OrderBy(d => d.Color).ThenBy(d => d.Location.Row).ThenBy(d => d.Location.Col);
    foreach (IDisplayable displayable in toDisplay)
    {
        Console.SetCursorPosition(displayable.Location.Col, displayable.Location.Row);
        Console.ForegroundColor = displayable.Color;
        Console.Write(displayable.ConsoleDisplayString);
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