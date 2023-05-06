using HercAndHippoLibCs;
using System.Diagnostics;

const int REFRESH_RATE = 20;

Stopwatch sw = new();

ConsoleKeyInfo keyInfo = default;
bool firstRender = true;

// Just for kicks,  record the gameplay for playback at the end.
// initialize history stack with 5 minutes' worth of history
Stack<Level> history = new(capacity: 1000 / REFRESH_RATE * 60 * 5);

Level curState = TestLevels.WallsLevel;
Level newState = curState;

sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_RATE);
    sw.Restart();

    newState = curState.RefreshCyclables(keyInfo);
    if (keyInfo.KeyChar == 'q') break;
    keyInfo = default;
    RefreshDisplay(curState, newState, firstRender);
    firstRender = false;
    curState = newState;
    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
    if (Console.KeyAvailable) 
        keyInfo = Console.ReadKey();  
}

// Play back the recording in reverse
while (history.Any())
{
    while (sw.ElapsedMilliseconds < REFRESH_RATE / 5) ;
    sw.Restart();
    newState = history.Pop();
    RefreshDisplay(curState, newState, forceRefresh: false);
    curState = newState;
}

// Helper Methods

void RefreshDisplay(Level oldState, Level newState, bool forceRefresh)
{
    if (forceRefresh) Console.Clear();
    if (!forceRefresh && newState.HasSameStateAs(oldState)) return;
    ClearOld(oldState, newState);
    ShowNew(oldState, newState, forceRefresh);
    history.Push(newState);
}

void ShowNew(Level oldState, Level newState, bool forceRefresh)
{
    IEnumerable<IDisplayable> toDisplay = newState.LevelObjects()
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
    foreach (IDisplayable toRemove in oldState.LevelObjects().Where(obj => !newState.LevelObjects().Contains(obj)))
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