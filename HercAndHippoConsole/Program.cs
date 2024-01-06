using HercAndHippoLibCs;
using HercAndHippoConsole;
using static HercAndHippoConsole.DisplayUtilities;
using System.Diagnostics;

const int MESSAGE_MARGIN = 3;
const int REFRESH_FREQUENCY_HZ = 50;

// Initialize data
Level state = DemoLevels.IntroducingTheHippo;
double averageCycleTime = ApproximateCycleTime(state);
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
displayPlan.GetDiffs(displayPlan);
displayPlan.RefreshDisplay();
ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");


// Main loop
while (true)
{
    futures = new(
        initialState: state, 
        scrollStatus: scrollStatus,
        bufferStats: bufferStats,
        mostRecentInput: lastAction, 
        averageCycleTime: averageCycleTime, 
        msPerCycle: cycleTimer.MillisecondsPerCycle); // plan for possible next states

    keyInfo = cycleTimer.AwaitCycle(); // Update once per 20 ms, return key input
    bufferStats.Refresh(); // Check if buffer size changed
    displayPlan = new(state, scrollStatus, bufferStats); // save current screen layout
    keyInfo = Console.KeyAvailable ? Console.ReadKey() : keyInfo; // Get next key input
    if (keyInfo.KeyChar == 'q') break; // Quit on q
    lastAction = keyInfo.ToActionInput();
    (state, scrollStatus, nextDisplayPlan) = futures.GetFuturePlan(lastAction);
    displayPlan.GetDiffs(nextDisplayPlan);
    refreshed = displayPlan.RefreshDisplay(); // Re-display anything that changed
    while (!refreshed) 
    {
        bufferStats.ForceRefresh();
        nextDisplayPlan = new DisplayPlan(state, scrollStatus, bufferStats);
        displayPlan.GetDiffs(nextDisplayPlan);
        refreshed = displayPlan.RefreshDisplay();
    }
    if (state.GetMessage() is Message message)
    {
        ShowMessage(message.Text);
    }
    else
    {
        ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
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

static double ApproximateCycleTime(Level level, int iterationCount = 10)
{
    Stopwatch sw = new();
    sw.Start();
    for (int i = 0; i < iterationCount; i++)
        level = level.RefreshCyclables(ActionInput.NoAction);
    sw.Stop();
    long totalTime = sw.ElapsedMilliseconds;
    return totalTime * 1.0 / iterationCount;
}