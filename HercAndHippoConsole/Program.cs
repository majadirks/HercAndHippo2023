using HercAndHippoLibCs;
using HercAndHippoConsole;
using System.Diagnostics;
using System.Runtime.CompilerServices;

const int REFRESH_INTERVAL_MS = 20;
const int MESSAGE_MARGIN = 4;
const int VIEW_MARGIN = 2;
const int HISTORY_SPEEDUP_FACTOR = 10;

Stopwatch sw = new();

ConsoleKeyInfo keyInfo = default;
bool forceRefresh = true;
bool bufferSizeChanged;

// Just for kicks,  record the gameplay for playback at the end.
// initialize history stack with 5 minutes' worth of history
Stack<Level> history = new(capacity: 1000 * 60 * 5 / REFRESH_INTERVAL_MS);

Level curState = TestLevels.WallsLevel;
Level newState;
IDisplayable[,] displayState = new IDisplayable[Console.BufferWidth - VIEW_MARGIN, Console.BufferHeight - VIEW_MARGIN]; // DisplayData(curState, Console.BufferWidth - VIEW_MARGIN, Console.BufferHeight - VIEW_MARGIN);

int bufferHeight = Console.BufferHeight;
int bufferWidth = Console.BufferWidth;


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
    forceRefresh = bufferSizeChanged;

    // Update current state to new state
    curState = newState;
    history.Push(newState);
    displayState = DisplayData(curState, Console.BufferWidth - VIEW_MARGIN, Console.BufferHeight - VIEW_MARGIN);

    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
    
    // Parse next keystroke
    if (Console.KeyAvailable) 
        keyInfo = Console.ReadKey();  
}

// Play back the recording in reverse
while (history.Any())
{
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS / HISTORY_SPEEDUP_FACTOR) ;
    sw.Restart();
    newState = history.Pop();
    RefreshDisplay(displayState, newState, forceRefresh: false);
    curState = newState;
    displayState = DisplayData(curState, Console.BufferWidth - VIEW_MARGIN, Console.BufferHeight - VIEW_MARGIN);
}

// Helper Methods
(bool changed, int BufferHeight, int BufferWidth) BufferSizeChanged(int bufferHeight, int bufferWidth)
{
    bool changed = bufferHeight != Console.BufferHeight || bufferWidth != Console.BufferWidth;
    return (changed, Console.BufferHeight, Console.BufferWidth);
}

void RefreshDisplay(IDisplayable[,] oldDisplay, Level newState, bool forceRefresh)
{
    // ToDo: fix potential bugs here when console changes size
    int maxCol() => Console.BufferWidth - VIEW_MARGIN;
    int maxRow() => Console.BufferHeight - VIEW_MARGIN;
    IDisplayable[,] newDisplay = DisplayData(newState, maxCol(), maxRow());
    for (int col = 0; col < maxCol(); col++)
    {
        for (int row = 0; row < maxRow(); row++)
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

void ShowMessage(string message)
{
    Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(message);
}