using HercAndHippoLibCs;
using HercAndHippoConsole;
using static HercAndHippoConsole.DisplayUtilities;
namespace HercAndHippoConsole;

internal class DisplayLoop
{
    public const int MESSAGE_MARGIN = 3;
    private readonly CycleTimer cycleTimer;
    private readonly BufferStats bufferStats;

    public Level State { get; private set; }
    private ScrollStatus scrollStatus;
    private DisplayPlan displayPlan;
    private ActionInputPair lastActions;
    private IEnumerable<DisplayDiff> diffs;
    public DisplayLoop(Level state, int frequency_hz)
    {
        if (frequency_hz < 1)
            throw new ArgumentException($"Frequency must be >=1, but was given {frequency_hz}");
        // Initialize data
        State = state;
        cycleTimer = new(frequencyHz: frequency_hz);
        scrollStatus = ScrollStatus.Default(state.Player.Location);
        bufferStats = new(bufferSizeChanged: true, bufferWidth: Console.BufferWidth, bufferHeight: Console.BufferHeight);
        displayPlan = new(state, scrollStatus, bufferStats);
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        lastActions = new(ActionInput.NoAction);

        // Initialize display
        ResetConsoleColors();
        diffs = displayPlan.GetDiffs(displayPlan);
        displayPlan.RefreshDisplay(diffs);
        ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
    }
    public void RunGame(GameController controller)
    {
        DisplayPlan nextDisplayPlan;
        FutureStates futures;
        bool refreshed;

        // Main loop
        while (State.WinState == WinState.InProgress)
        {
            futures = new(
                initialPlan: displayPlan,
                initialState: State,
                scrollStatus: scrollStatus,
                bufferStats: bufferStats,
                mostRecentInputs: lastActions); // plan for possible next states
            cycleTimer.AwaitCycle(); // Update once per 20 ms, return key input
            bufferStats.Refresh(); // Check if buffer size changed
            displayPlan = new(State, scrollStatus, bufferStats); // save current screen layout
            lastActions = controller.NextAction(State);
            if (lastActions.Where(a => a == ActionInput.Quit).Any()) break;
            (State, scrollStatus, diffs) = futures.GetFutureDiffs(lastActions);
            refreshed = displayPlan.RefreshDisplay(diffs); // Re-display anything that changed

            while (!refreshed)
            {
                bufferStats.ForceRefresh();
                nextDisplayPlan = new DisplayPlan(State, scrollStatus, bufferStats);
                diffs = displayPlan.GetDiffs(nextDisplayPlan);
                refreshed = displayPlan.RefreshDisplay(diffs);
            }
            UpdateMessageFromLevel(State);
        }
        ResetConsoleColors(); // Clean up

        if (State.WinState == WinState.Won)
            Console.WriteLine("Huzzah! You have won!");
        else
            Console.WriteLine("You lost! Try again!");
        Console.ReadLine();
    }

    private static void ShowMessage(string message)
    {
        ResetConsoleColors();
        Console.SetCursorPosition(1, Console.BufferHeight - MESSAGE_MARGIN);
        ClearCurrentConsoleLine();
        Console.WriteLine(message);
    }

    private static void UpdateMessageFromLevel(Level state)
    {
        if (state.GetMessage() is Message message)
        {
            ShowMessage(message.Text);
        }
        else
        {
            //ShowMessage(FutureStates.GetCacheStats().ToString());
            ShowMessage("Use arrow keys to move, shift + arrow keys to shoot, 'q' to quit.");
        }
    }

    private static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
}
