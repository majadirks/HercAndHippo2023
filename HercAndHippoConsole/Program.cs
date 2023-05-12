using HercAndHippoLibCs;
using HercAndHippoConsole;
using static HercAndHippoConsole.Constants;
using static HercAndHippoConsole.DisplayPlan;
using System.Diagnostics;

const int MESSAGE_MARGIN = 3;
const int REFRESH_INTERVAL_MS = 20;

Stopwatch sw = new();
ConsoleKeyInfo keyInfo = default;

Level state = TestLevels.WallsLevel;
ScrollStatus scrollStatus = ScrollStatus.Default with { LogicalCenter = state.Player.Location };
BufferStats bufferStats = new(BufferSizeChanged: true, BufferWidth: Console.BufferWidth, BufferHeight: Console.BufferHeight);
DisplayPlan oldDisplay = CreateDisplayPlan(state, scrollStatus, bufferStats);
DisplayPlan newDisplay;

RefreshDisplay(oldDisplay, oldDisplay, bufferStats);
sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS);
    sw.Restart();

    // Check if buffer size changed
    bufferStats = bufferStats.Refresh();
    oldDisplay = CreateDisplayPlan(state, scrollStatus, bufferStats);

    // React to any key input
    if (Console.KeyAvailable) keyInfo = Console.ReadKey();
    if (keyInfo.KeyChar == 'q') break;
    state = state.RefreshCyclables(keyInfo.ToActionInput());
    keyInfo = default;

    // Check if we need to move the focus of the screen
    scrollStatus = scrollStatus
        .UpdateTriggerRadius(bufferStats)
        .DoScroll(state.Player.Location, bufferStats);
        
    // Display current state
    newDisplay = CreateDisplayPlan(state, scrollStatus, bufferStats);
    RefreshDisplay(oldDisplay, newDisplay, bufferStats);

    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");    
}


// Helper Methods
void RefreshDisplay(DisplayPlan oldDisplayPlan, DisplayPlan newDisplayPlan, BufferStats bufferStats)
{
    bool forceRefresh = bufferStats.BufferSizeChanged;

    var oldDisplay = oldDisplayPlan.PlanArray;
    var newDisplay = newDisplayPlan.PlanArray;

    int maxCol = (forceRefresh ? Console.BufferWidth : bufferStats.BufferWidth) - VIEW_MARGIN;
    int maxRow = (forceRefresh ? Console.BufferHeight : bufferStats.BufferHeight) - VIEW_MARGIN;
    ;
    // Rather than using the cached maxCol and maxRow values calculated above,
    // the following method recalculates the buffer width and height when it is needed
    // to prevent attempting to set the cursor position to an offscreen location (which throws an exception).
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