using HercAndHippoLibCs;
using HercAndHippoConsole;
using System.Diagnostics;

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
IDisplayable[,] oldDisplay = new IDisplayable[Console.BufferWidth - VIEW_MARGIN, Console.BufferHeight - VIEW_MARGIN]; // DisplayData(curState, Console.BufferWidth - VIEW_MARGIN, Console.BufferHeight - VIEW_MARGIN);
IDisplayable[,] newDisplay = oldDisplay;
int bufferHeight = Console.BufferHeight;
int bufferWidth = Console.BufferWidth;


sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS);
    sw.Restart();

    // Parse key input
    if (Console.KeyAvailable) keyInfo = Console.ReadKey();
    if (keyInfo.KeyChar == 'q') break;
    newState = curState.RefreshCyclables(keyInfo.ToActionInput());
    keyInfo = default;

    //  Decide if we need to refresh
    (bufferSizeChanged, bufferHeight, bufferWidth) = BufferSizeChanged(bufferHeight, bufferWidth);
    forceRefresh = bufferSizeChanged;

    // Display current state
    oldDisplay = DisplayData(curState, bufferWidth, bufferHeight);
    newDisplay = DisplayData(newState, bufferWidth, bufferHeight);
    RefreshDisplay(oldDisplay, newDisplay, forceRefresh, bufferWidth, bufferHeight);

    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");

    // Update current state to new state
    curState = newState;
    history.Push(newState);

}

// Play back the recording in reverse
//while (history.Any())
//{
//    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS / HISTORY_SPEEDUP_FACTOR) ;
//    sw.Restart();
//    newState = history.Pop();
//    RefreshDisplay(oldDisplay, newState, forceRefresh: false, bufferWidth, bufferHeight);
//    curState = newState;
//    oldDisplay = DisplayData(curState, bufferHeight - VIEW_MARGIN, bufferHeight - VIEW_MARGIN);
//}

// Helper Methods
(bool changed, int BufferHeight, int BufferWidth) BufferSizeChanged(int bufferHeight, int bufferWidth)
{
    bool changed = bufferHeight != Console.BufferHeight || bufferWidth != Console.BufferWidth;
    return (changed, Console.BufferHeight, Console.BufferWidth);
}

void RefreshDisplay(IDisplayable[,] oldDisplay, IDisplayable[,] newDisplay, bool forceRefresh, int bufferWidth, int bufferHeight)
{
    int maxCol = (forceRefresh? Console.BufferWidth : bufferWidth) - VIEW_MARGIN;
    int maxRow = (forceRefresh? Console.BufferHeight : bufferHeight) - VIEW_MARGIN;
    bool InView(int col, int row) => col + 1 < Console.BufferWidth - VIEW_MARGIN && row + 1 < Console.BufferHeight - VIEW_MARGIN;

    for (int row = 0; row < maxRow; row++)
    {
        for (int col = 0; col < maxCol; col++)
        {
            IDisplayable oldDisp = oldDisplay[col, row];
            IDisplayable newDisp = newDisplay[col, row];
            if (oldDisp != newDisp && newDisp != default && InView(col, row))
            {
                // Something is here that wasn't here before, so show it
                Console.SetCursorPosition(col + 1, row + 1);
                Console.ForegroundColor = newDisp.Color;
                Console.Write(newDisp.ConsoleDisplayString);
            }
            if ((forceRefresh || (newDisp == default && oldDisp != default)) &&
                InView(col,row))
            {
                // Something used to be here, but now nothing is here, so clear the spot
                Console.SetCursorPosition(col + 1, row + 1);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" ");
            }
        }
        
    }
}

IDisplayable[,] DisplayData(Level state, int bufferWidth, int bufferHeight) 
{
    IDisplayable[,] ToShow = new IDisplayable[bufferWidth, bufferHeight];
    int playerRow = state.Player.Location.Row;
    int playerCol = state.Player.Location.Col;

    int minCol = 1;
    int minRow = 1;

    int midCol = (bufferWidth - VIEW_MARGIN)/ 2;
    int midRow = (bufferHeight - VIEW_MARGIN) / 2;

    foreach (IDisplayable toShow in state.LevelObjects)
    {
        int writeCol = midCol + toShow.Location.Col - playerCol;
        int writeRow = midRow + toShow.Location.Row - playerRow;

        if (writeCol >= minCol && writeCol <= bufferWidth && writeRow >= minRow && writeRow <= bufferHeight)
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