using HercAndHippoLibCs;
using HercAndHippoConsole;
using static HercAndHippoConsole.DisplayPlan;
using System.Diagnostics;

const int MESSAGE_MARGIN = 3;
const int REFRESH_INTERVAL_MS = 20;

// Initialize data
Stopwatch sw = new();
ConsoleKeyInfo keyInfo = default;
Level state = TestLevels.WallsLevel;
ScrollStatus scrollStatus = ScrollStatus.Default with { LogicalCenter = state.Player.Location };
BufferStats bufferStats = new(BufferSizeChanged: true, BufferWidth: Console.BufferWidth, BufferHeight: Console.BufferHeight);
DisplayPlan displayPlan = CreateDisplayPlan(state, scrollStatus, bufferStats);

displayPlan.RefreshDisplay(state, scrollStatus, bufferStats);
sw.Start();
while (true)
{
    while (sw.ElapsedMilliseconds < REFRESH_INTERVAL_MS);
    sw.Restart();

    // Check if buffer size changed
    bufferStats = bufferStats.Refresh();
    displayPlan = CreateDisplayPlan(state, scrollStatus, bufferStats);

    // React to any key input
    if (Console.KeyAvailable) keyInfo = Console.ReadKey();
    if (keyInfo.KeyChar == 'q') break;
    state = state.RefreshCyclables(keyInfo.ToActionInput());
    keyInfo = default;

    // Check if we need to move the focus of the screen
    scrollStatus = scrollStatus.Update(state.Player.Location, bufferStats);
        
    // Display current state
    displayPlan.RefreshDisplay(state, scrollStatus, bufferStats);

    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");    
}

static void ShowMessage(string message)
{
    Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(message);
}