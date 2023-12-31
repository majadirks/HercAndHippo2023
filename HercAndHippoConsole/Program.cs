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
Console.OutputEncoding = System.Text.Encoding.UTF8;
ActionInput lastAction = ActionInput.NoAction;

ConsoleKeyInfo keyInfo;
FutureStates futures;

// Initialize display
ResetConsoleColors();
displayPlan.RefreshDisplay(state, scrollStatus);
ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");


// Main loop
while (true)
{
    futures = new(
        state: state, 
        mostRecent: lastAction, 
        averageCycleTime: averageCycleTime, 
        msPerCycle: cycleTimer.MillisecondsPerCycle); // calculate possible next states

    cycleTimer.AwaitCycle(); // Update once per 20 ms
    bufferStats.Refresh(); // Check if buffer size changed
    displayPlan = new(state, scrollStatus, bufferStats); // save current screen layout
    keyInfo = Console.KeyAvailable ? Console.ReadKey() : default; // Get next key input
    if (keyInfo.KeyChar == 'q') break; // Quit on q
    lastAction = keyInfo.ToActionInput();
    state = futures.GetState(lastAction); // Update level state using key input
    scrollStatus = scrollStatus.Update(state.Player.Location, bufferStats); // Plan to scroll screen if needed.
    displayPlan.RefreshDisplay(newState: state, newScrollStatus: scrollStatus); // Re-display anything that changed
}
ResetConsoleColors(); // Clean up

// Helper Functions
static void ShowMessage(string message)
{
    ResetConsoleColors();
    Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
    Console.WriteLine(message);
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