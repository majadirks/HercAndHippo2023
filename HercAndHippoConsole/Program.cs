using HercAndHippoLibCs;
using HercAndHippoConsole;
using System.Diagnostics;
using System.Runtime.CompilerServices;

const int REFRESH_INTERVAL_MS = 20;
const int MESSAGE_MARGIN = 3;
const int VIEW_MARGIN = 2;
const int HISTORY_SPEEDUP_FACTOR = 10;

Stopwatch sw = new();

ConsoleKeyInfo keyInfo = default;
bool forceRefresh = true;
bool playerMoved = false;
bool bufferSizeChanged = false;

// Just for kicks,  record the gameplay for playback at the end.
// initialize history stack with 5 minutes' worth of history
Stack<Level> history = new(capacity: 1000 * 60 * 5 / REFRESH_INTERVAL_MS);

Level curState = TestLevels.WallsLevel;
Level newState = curState;

int bufferHeight = Console.BufferHeight;
int bufferWidth = Console.BufferWidth;

Column playerCol = curState.Player.Location.Col;
Row playerRow = curState.Player.Location.Row;

sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS);
    sw.Restart();

    newState = curState.RefreshCyclables(keyInfo.ToActionInput());
    if (keyInfo.KeyChar == 'q') break;
    keyInfo = default;
    RefreshDisplay(curState, newState, forceRefresh);
    (bufferSizeChanged, bufferHeight, bufferWidth)  = BufferSizeChanged(bufferHeight, bufferWidth);
    (playerMoved, playerCol, playerRow) = PlayerMoved(newState, playerCol, playerRow);
    forceRefresh = bufferSizeChanged || playerMoved;
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

(bool playerMoved, Column newCol, Row newRow) PlayerMoved(Level state, Column playerCol, Row playerRow)
{
    Column newCol = state.Player.Location.Col;
    Row newRow = state.Player.Location.Row;
    bool moved = playerCol != newCol || playerRow != newRow;
    return (moved, newCol, newRow);
}

void RefreshDisplay(Level oldState, Level newState, bool forceRefresh)
{
    if (forceRefresh) Console.Clear();
    if (!forceRefresh && newState == oldState) return;
    ShowNew(oldState, newState);
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
        int colOffset = ColOffset(col: displayable.Location.Col, playerCol: newState.Player.Location.Col);
        int rowOffset = RowOffset(row: displayable.Location.Row, playerRow: newState.Player.Location.Row);
        PutString(col: displayable.Location.Col, row:displayable.Location.Row, 
            colOffset: colOffset, rowOffset: rowOffset,
            color: displayable.Color, msg: displayable.ConsoleDisplayString);
    }
}

void ClearOld(Level oldState, Level newState, bool forceRefresh)
{
    foreach (IDisplayable toRemove in oldState.LevelObjects.Where(obj => forceRefresh || !newState.LevelObjects.Contains(obj)))
    {
        int colOffset = ColOffset(col: toRemove.Location.Col, playerCol: oldState.Player.Location.Col);
        int rowOffset = RowOffset(row: toRemove.Location.Row, playerRow: oldState.Player.Location.Row);
        PutString(col: toRemove.Location.Col, row: toRemove.Location.Row,
                  colOffset: colOffset, rowOffset: rowOffset,
                  color: ConsoleColor.Black, msg: " ") ;
    }
}

void ShowMessage(string message)
{
    Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(message);
}

void PutString(int col, int row, int colOffset, int rowOffset, ConsoleColor color, string msg)
{
    int minCol = 1;
    int minRow = 1;

    int midCol = Console.BufferWidth / 2;
    int midRow = Console.BufferHeight / 2;
    
    int maxCol = Console.BufferWidth - VIEW_MARGIN;
    int maxRow = Console.BufferHeight - VIEW_MARGIN;

    int writeCol = midCol + colOffset;
    int writeRow = midRow + rowOffset;

    if (writeRow > maxRow || writeRow < minRow || writeCol > maxCol || writeCol < minCol) return; // Out-of-view, so don't write anything
    Console.SetCursorPosition(writeCol, writeRow);
    Console.ForegroundColor = color;
    Console.WriteLine(msg);
}

int ColOffset(int col, int playerCol) => col - playerCol;
int RowOffset(int row, int playerRow) =>  row - playerRow;