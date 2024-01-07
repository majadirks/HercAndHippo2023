using HercAndHippoLibCs;
using HercAndHippoConsole;
using static HercAndHippoConsole.DisplayUtilities;
using System.Diagnostics;

const int MESSAGE_MARGIN = 3;
const int REFRESH_FREQUENCY_HZ = 40;

// Initialize data
Level state = DemoLevels.ManyObjectsStressTest();

CycleTimer cycleTimer = new(frequencyHz: REFRESH_FREQUENCY_HZ);
ScrollStatus scrollStatus = ScrollStatus.Default(state.Player.Location);
BufferStats bufferStats = new(bufferSizeChanged: true, bufferWidth: Console.BufferWidth, bufferHeight: Console.BufferHeight);
DisplayPlan displayPlan = new(state, scrollStatus, bufferStats);
DisplayPlan nextDisplayPlan;
Console.OutputEncoding = System.Text.Encoding.UTF8;
ActionInput lastAction = ActionInput.NoAction;

ConsoleKeyInfo keyInfo;
FutureStates futures;


// Initialize display
ResetConsoleColors();
bool refreshed;
IEnumerable<DisplayDiff> diffs = displayPlan.GetDiffs(displayPlan);
displayPlan.RefreshDisplay(diffs);
ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");

// Main loop
while (true)
{
    futures = new(
        initialPlan: displayPlan,
        initialState: state, 
        scrollStatus: scrollStatus,
        bufferStats: bufferStats,
        mostRecentInput: lastAction); // plan for possible next states
    keyInfo = cycleTimer.AwaitCycle(); // Update once per 20 ms, return key input
    bufferStats.Refresh(); // Check if buffer size changed
    displayPlan = new(state, scrollStatus, bufferStats); // save current screen layout
    keyInfo = Console.KeyAvailable ? Console.ReadKey() : keyInfo; // Get next key input
    if (keyInfo.KeyChar == 'q') break; // Quit on q
    lastAction = keyInfo.ToActionInput();
    (state, scrollStatus, diffs) = futures.GetFutureDiffs(lastAction);;
    refreshed = displayPlan.RefreshDisplay(diffs); // Re-display anything that changed
    
    while (!refreshed) 
    {
        bufferStats.ForceRefresh();
        nextDisplayPlan = new DisplayPlan(state, scrollStatus, bufferStats);
        diffs = displayPlan.GetDiffs(nextDisplayPlan);
        refreshed = displayPlan.RefreshDisplay(diffs);
    }
    if (state.GetMessage() is Message message)
    {
        ShowMessage(message.Text);
    }
    else
    {
        ShowMessage(FutureStates.GetCacheStats().ToString());
        //ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
    }
}
ResetConsoleColors(); // Clean up

// Helper Functions
static void ShowMessage(string message)
{
    ResetConsoleColors();
    Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
    ClearCurrentConsoleLine();
    Console.WriteLine(message);
}

static void ClearCurrentConsoleLine()
{
    int currentLineCursor = Console.CursorTop;
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, currentLineCursor);
}
