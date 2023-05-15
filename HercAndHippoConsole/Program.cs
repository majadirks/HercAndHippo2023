using HercAndHippoLibCs;
using HercAndHippoConsole;

const int MESSAGE_MARGIN = 3;
const int REFRESH_FREQUENCY_HZ = 50;

// Initialize data
CycleTimer cycleTimer = new(frequencyHz: REFRESH_FREQUENCY_HZ);
ConsoleKeyInfo keyInfo;
Level state = DemoLevels.SoManyBullets();
ScrollStatus scrollStatus = ScrollStatus.Default(state.Player.Location);
BufferStats bufferStats = new(bufferSizeChanged: true, bufferWidth: Console.BufferWidth, bufferHeight: Console.BufferHeight);
DisplayPlan displayPlan = new(state, scrollStatus, bufferStats);
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Initialize display
ResetConsoleColors();
displayPlan.RefreshDisplay(state, scrollStatus);
ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");

// Main loop
while (true)
{
    cycleTimer.AwaitCycle(); // Update only once per 20 ms
    bufferStats.Refresh(); // Check if buffer size changed
    displayPlan = new(state, scrollStatus, bufferStats); // save current screen layout
    keyInfo = GetKeyIfAny(); // Get next key input
    if (keyInfo.KeyChar == 'q') break; // Quit on q
    state = state.RefreshCyclables(keyInfo.ToActionInput()); // Update level state using key input
    scrollStatus = scrollStatus.Update(state.Player.Location, bufferStats); // Plan to scroll screen if needed
    displayPlan.RefreshDisplay(newState: state, newScrollStatus: scrollStatus); // Re-display anything that changed
    Console.SetCursorPosition(1, Console.BufferHeight - 1); // Move the cursor so it doesn't always appear next to the player
}
ResetConsoleColors(); // Clean up

// Helper Functions
static void ShowMessage(string message)
{
    ResetConsoleColors();
    Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
    Console.WriteLine(message);
}

static ConsoleKeyInfo GetKeyIfAny()
{
    ConsoleKeyInfo nextKey = default;
    if (Console.KeyAvailable) nextKey = Console.ReadKey();
    return nextKey;
}
static void ResetConsoleColors()
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.BackgroundColor = ConsoleColor.Black;
}

