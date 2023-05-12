using HercAndHippoLibCs;
using HercAndHippoConsole;
using static HercAndHippoConsole.Constants;
using static HercAndHippoConsole.DisplayLogic;
using System.Diagnostics;

const int MESSAGE_MARGIN = 3;
const int REFRESH_INTERVAL_MS = 20;

Stopwatch sw = new();
ConsoleKeyInfo keyInfo = default;
bool forceRefresh;
bool bufferSizeChanged;

Level oldState = TestLevels.WallsLevel;
Location oldLogicalCenter = oldState.Player.Location;
ScrollStatus scrollStatus = ScrollStatus.Default with { LogicalCenter = oldLogicalCenter };
IDisplayable[,] oldDisplay = DisplayData(oldState, scrollStatus, Console.BufferWidth, Console.BufferHeight);
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

    oldDisplay = DisplayData(oldState, scrollStatus, bufferWidth, bufferHeight);

    // Parse key input
    if (Console.KeyAvailable) keyInfo = Console.ReadKey();
    if (keyInfo.KeyChar == 'q') break;
    newState = oldState.RefreshCyclables(keyInfo.ToActionInput());
    keyInfo = default;

    //  Decide if we need to refresh
    (bufferSizeChanged, bufferHeight, bufferWidth) = BufferSizeChanged(bufferHeight, bufferWidth);
    forceRefresh = bufferSizeChanged;

    // Check if we need to move the focus of the screen
    scrollStatus = scrollStatus
        .UpdateTriggerRadius(bufferWidth, bufferHeight)
        .DoScroll(newState.Player.Location, oldLogicalCenter, bufferWidth, bufferHeight);
        
    // Display current state
    newDisplay = DisplayData(newState, scrollStatus, bufferWidth, bufferHeight);
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
    bool InView(int col, int row) => col < Console.BufferWidth - VIEW_MARGIN && row < Console.BufferHeight - VIEW_MARGIN;

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
                Console.SetCursorPosition(col, row);
                Console.ForegroundColor = newDisp.Color;
                Console.Write(newDisp.ConsoleDisplayString);
            }
            if ((newDisp == default && (forceRefresh || oldDisp != default)) &&
                InView(col, row))
            {
                // Something used to be here, but now nothing is here, so clear the spot
                Console.SetCursorPosition(col, row);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" ");
            }
        }      
    }
}


void ShowMessage(string message)
{
    Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(message);
}