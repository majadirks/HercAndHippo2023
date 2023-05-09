using HercAndHippoLibCs;
using HercAndHippoConsole;
using System.Diagnostics;

const int REFRESH_INTERVAL_MS = 20;
const int MESSAGE_MARGIN = 3;
const int VIEW_MARGIN = 2;
const int HISTORY_SPEEDUP_FACTOR = 10;

Stopwatch sw = new();

ConsoleKeyInfo keyInfo = default;
bool forceRefresh = true;

// Just for kicks,  record the gameplay for playback at the end.
// initialize history stack with 5 minutes' worth of history
Stack<Level> history = new(capacity: 1000 * 60 * 5 / REFRESH_INTERVAL_MS);

Level curState = TestLevels.WallsLevel;
Level newState = curState;

int bufferHeight = Console.BufferHeight;
int bufferWidth = Console.BufferWidth;  

sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS);
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
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS / HISTORY_SPEEDUP_FACTOR) ;
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
    ShowNew(oldState, newState);
    history.Push(newState);
}

void ShowNew(Level oldState, Level newState)
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
    => WriteIfInView(1, Console.BufferHeight - MESSAGE_MARGIN, ConsoleColor.White, message);

void WriteIfInView(int col, int row, ConsoleColor color, string msg)
{
    int maxCol = Console.BufferWidth - VIEW_MARGIN;
    int maxRow = Console.BufferHeight - VIEW_MARGIN;
    if (row > maxRow || col > maxCol) return; // Out-of-view, so don't write anything
    Console.SetCursorPosition(col, row);
    Console.ForegroundColor = color;
    Console.WriteLine(msg);
}