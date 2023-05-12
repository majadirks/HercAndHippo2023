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

oldDisplay.RefreshDisplay(oldDisplay, bufferStats);
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
    oldDisplay.RefreshDisplay(newDisplay, bufferStats);

    ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");    
}


// Helper Methods
void ShowMessage(string message)
{
    Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(message);
}