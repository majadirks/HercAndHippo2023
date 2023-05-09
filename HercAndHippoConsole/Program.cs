using HercAndHippoLibCs;
using HercAndHippoConsole;
using System.Diagnostics;

const int REFRESH_RATE = 20;

Stopwatch sw = new();

ConsoleKeyInfo keyInfo = default;
bool forceRefresh = true;

// Just for kicks,  record the gameplay for playback at the end.
// initialize history stack with 5 minutes' worth of history
Stack<Level> history = new(capacity: 1000 / REFRESH_RATE * 60 * 5);

Level curState = TestLevels.WallsLevel;
Level newState = curState;

int bufferHeight = Console.BufferHeight;
int bufferWidth = Console.BufferWidth;  

sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_RATE);
    sw.Restart();

    newState = curState.RefreshCyclables(keyInfo.ToActionInput());
    if (keyInfo.KeyChar == 'q') break;
    keyInfo = default;
    RefreshDisplay(curState, newState, forceRefresh);
    (forceRefresh, bufferHeight, bufferWidth)  = BufferSizeChanged(bufferHeight, bufferWidth);
    curState = newState;
    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
    if (Console.KeyAvailable) 
        keyInfo = Console.ReadKey();  
}

// Play back the recording in reverse
while (history.Any())
{
    while (sw.ElapsedMilliseconds < REFRESH_RATE / 10) ;
    sw.Restart();
    newState = history.Pop();
    RefreshDisplay(curState, newState, forceRefresh: false);
    curState = newState;
}

// Helper Methods

(bool changed, int BufferHeight, int BufferWidth) BufferSizeChanged(int bufferHeight, int bufferWidth)
{
    bool changed = bufferHeight != Console.BufferHeight || bufferWidth != Console.BufferWidth;
    return (changed, Console.BufferHeight, Console.BufferWidth);
}

void RefreshDisplay(Level oldState, Level newState, bool forceRefresh)
{
    if (forceRefresh) Console.Clear();
    if (!forceRefresh && newState == oldState) return;
    ClearOld(oldState, newState, forceRefresh);
    ShowNew(oldState, newState, forceRefresh);
    history.Push(newState);
}

void ShowNew(Level oldState, Level newState, bool forceRefresh)
{
    IEnumerable<IDisplayable> toDisplay = newState.LevelObjects
        .OrderBy(d => d.Color).ThenBy(d => d.Location.Row).ThenBy(d => d.Location.Col);
    foreach (IDisplayable displayable in toDisplay)
    {
        WriteIfInView(displayable.Location.Col, displayable.Location.Row, displayable.Color, displayable.ConsoleDisplayString);
    }
}

void ClearOld(Level oldState, Level newState, bool forceRefresh)
{
    foreach (IDisplayable toRemove in oldState.LevelObjects.Where(obj => forceRefresh || !newState.LevelObjects.Contains(obj)))
    {
        WriteIfInView(toRemove.Location.Col, toRemove.Location.Row, ConsoleColor.Black, " ");
    }
}

void ShowMessage(string message) 
    => WriteIfInView(1, Console.BufferHeight - 3, ConsoleColor.White, message);

void WriteIfInView(int col, int row, ConsoleColor color, string msg)
{
    int maxCol = Console.BufferWidth - 2;
    int maxRow = Console.BufferHeight - 2;
    if (row > maxRow || col > maxCol) return;
    Console.SetCursorPosition(col, row);
    Console.ForegroundColor = color;
    Console.WriteLine(msg);
}