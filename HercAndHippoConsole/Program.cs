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
IDisplayable[,] displayState = DisplayData(curState, Console.BufferWidth - VIEW_MARGIN, Console.BufferHeight - VIEW_MARGIN);

int bufferHeight = Console.BufferHeight;
int bufferWidth = Console.BufferWidth;

//Column playerCol = curState.Player.Location.Col;
//Row playerRow = curState.Player.Location.Row;

sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS);
    sw.Restart();

    // Parse key input
    if (keyInfo.KeyChar == 'q') break;
    newState = curState.RefreshCyclables(keyInfo.ToActionInput());
    keyInfo = default;

    // Display current state
    RefreshDisplay(displayState, newState, forceRefresh);

    //  Decide if we need to refresh
    (bufferSizeChanged, bufferHeight, bufferWidth)  = BufferSizeChanged(bufferHeight, bufferWidth);
    //(playerMoved, playerCol, playerRow) = PlayerMoved(newState, playerCol, playerRow);
    forceRefresh = bufferSizeChanged;

    // Update current state to new state
    curState = newState;
    displayState = DisplayData(curState, Console.BufferWidth - VIEW_MARGIN, Console.BufferHeight - VIEW_MARGIN);

    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
    
    // Parse next keystroke
    if (Console.KeyAvailable) 
        keyInfo = Console.ReadKey();  
}

// Play back the recording in reverse
//while (history.Any())
//{
//    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS / HISTORY_SPEEDUP_FACTOR) ;
//    sw.Restart();
//    newState = history.Pop();
//    RefreshDisplay(curState, newState, forceRefresh: false);
//    curState = newState;
//}

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

void RefreshDisplay(IDisplayable[,] oldDisplay, Level newState, bool forceRefresh)
{
    //if (forceRefresh) Console.Clear();
    //if (!forceRefresh && newState == oldState) return;
    //ShowNew(oldState, newState);
    //ClearOld(oldState, newState, forceRefresh);
    //ShowNew(oldState, newState);
    //history.Push(newState);

    int maxCol = Console.BufferWidth - VIEW_MARGIN;
    int maxRow = Console.BufferHeight - VIEW_MARGIN;
    IDisplayable[,] newDisplay = DisplayData(newState, maxCol, maxRow);
    for (int col = 0; col < maxCol; col++)
    {
        for (int row = 0; row < maxRow; row++)
        {
            IDisplayable oldDisp = oldDisplay[col, row];
            IDisplayable newDisp = newDisplay[col, row];
            if (newDisp == default && oldDisp != default)
            {
                // Something used to be here, but now nothing is here, so clear the spot
                Console.SetCursorPosition(col + 1, row + 1);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" ");
            }
            if (oldDisp != newDisp && newDisp != default)
            {
                // There is something here that wasn't here before, so show it.
                IDisplayable toDisp = newDisplay[col, row];
                Console.SetCursorPosition(col + 1, row + 1);
                Console.ForegroundColor = toDisp.Color;
                Console.WriteLine(toDisp.ConsoleDisplayString);
            } 
        }
    }
}

IDisplayable[,] DisplayData(Level state, int maxCol, int maxRow) 
{
    IDisplayable[,] ToShow = new IDisplayable[maxCol, maxRow];
    int playerRow = state.Player.Location.Row;
    int playerCol = state.Player.Location.Col;

    int minCol = 1;
    int minRow = 1;

    int midCol = maxCol / 2;
    int midRow = maxRow / 2;

    foreach (IDisplayable toShow in state.LevelObjects)
    {
        int writeCol = midCol + toShow.Location.Col - playerCol;
        int writeRow = midRow + toShow.Location.Row - playerRow;

        if (writeCol >= minCol && writeCol <= maxCol && writeRow >= minRow && writeRow <= maxRow)
        {
            ToShow[writeCol - 1, writeRow - 1] = toShow;
        }
    }
    return ToShow;
}

void ShowNew(Level oldState, Level newState)
{
    IEnumerable<IDisplayable> toDisplay = newState.LevelObjects
        .OrderBy(d => d.Color).ThenBy(d => d.Location.Row).ThenBy(d => d.Location.Col);
    foreach (IDisplayable displayable in toDisplay)
    {
        int colOffset = ColOffset(col: displayable.Location.Col, playerCol: newState.Player.Location.Col);
        int rowOffset = RowOffset(row: displayable.Location.Row, playerRow: newState.Player.Location.Row);
        PutStringWithOffset(col: displayable.Location.Col, row:displayable.Location.Row, 
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
        PutStringWithOffset(col: toRemove.Location.Col, row: toRemove.Location.Row,
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

void PutStringWithOffset(int col, int row, int colOffset, int rowOffset, ConsoleColor color, string msg)
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

void PutString(int col, int row, ConsoleColor color, string msg)
{
    int minCol = 1;
    int minRow = 1;

    int maxCol = Console.BufferWidth - VIEW_MARGIN;
    int maxRow = Console.BufferHeight - VIEW_MARGIN;


    if (row > maxRow || row < minRow || col > maxCol || col < minCol) return; // Out-of-view, so don't write anything
    Console.SetCursorPosition(col, row);
    Console.ForegroundColor = color;
    Console.WriteLine(msg);
}

int ColOffset(int col, int playerCol) => col - playerCol;
int RowOffset(int row, int playerRow) =>  row - playerRow;