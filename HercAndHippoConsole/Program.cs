using HercAndHippoLibCs;
using HercAndHippoConsole;
using System.Diagnostics;
using static System.Math;

const int REFRESH_INTERVAL_MS = 20;
const int MESSAGE_MARGIN = 4;
const int VIEW_MARGIN = 2;

Stopwatch sw = new();

ConsoleKeyInfo keyInfo = default;
bool forceRefresh;
bool bufferSizeChanged;

Level oldState = TestLevels.WallsLevel;
IDisplayable[,] oldDisplay = DisplayData(oldState, Console.BufferWidth, Console.BufferHeight);
Level newState;
IDisplayable[,] newDisplay;
int bufferHeight = Console.BufferHeight;
int bufferWidth = Console.BufferWidth;


RefreshDisplay(oldDisplay, oldDisplay, forceRefresh: true, bufferWidth: bufferWidth, bufferHeight: bufferHeight);
sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS);
    sw.Restart();

    // Parse key input
    if (Console.KeyAvailable) keyInfo = Console.ReadKey();
    if (keyInfo.KeyChar == 'q') break;
    newState = oldState.RefreshCyclables(keyInfo.ToActionInput());
    keyInfo = default;

    //  Decide if we need to refresh
    (bufferSizeChanged, bufferHeight, bufferWidth) = BufferSizeChanged(bufferHeight, bufferWidth);
    forceRefresh = bufferSizeChanged;

    // Display current state
    oldDisplay = DisplayData(oldState, bufferWidth, bufferHeight);
    newDisplay = DisplayData(newState, bufferWidth, bufferHeight);
    RefreshDisplay(oldDisplay, newDisplay, forceRefresh, bufferWidth, bufferHeight);

    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");

    // Update current state to new state
    oldState = newState;
}


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
            if (( newDisp!= default && (forceRefresh || (oldDisp != newDisp))) && 
                InView(col, row))
            {
                // Something is here that wasn't here before, so show it
                Console.SetCursorPosition(col + 1, row + 1);
                Console.ForegroundColor = newDisp.Color;
                Console.Write(newDisp.ConsoleDisplayString);
            }
            if ((newDisp == default && (forceRefresh || oldDisp != default)) &&
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

    Column minCol = 1;
    Row minRow = 1;

    Column screenCenterCol = (bufferWidth - VIEW_MARGIN) / 2;
    Row screenCenterRow = (bufferHeight - VIEW_MARGIN) / 2;

    Column logicalCenterCol = Max(Min(state.Player.Location.Col, bufferWidth - screenCenterCol), minCol);
    Row logicalCenterRow = Max(Min(state.Player.Location.Row, bufferHeight- screenCenterRow), minRow);
        
    foreach (IDisplayable toShow in state.LevelObjects)
    {      
        Column writeCol = screenCenterCol + toShow.Location.Col - logicalCenterCol;
        Row writeRow = screenCenterRow + toShow.Location.Row - logicalCenterRow;
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