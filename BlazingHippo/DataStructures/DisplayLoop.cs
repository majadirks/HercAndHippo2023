using BlazingHippo.Pages;
using HercAndHippoLibCs;
namespace BlazingHippo;

internal class DisplayLoop
{
    public const int MESSAGE_MARGIN = 3;
    private readonly CancellationTokenSource cts;
    private readonly IProgress<bool> cycleProgress;
    private readonly PlayGame display;
    private readonly StatusBar statusBar;
    private readonly GameController controller;
    public Level State { get; private set; }
    private ScrollStatus scrollStatus;
    private DisplayPlan displayPlan;
    private ActionInputPair lastActions;
    private IEnumerable<DisplayDiff> diffs;
    public DisplayLoop(GameController controller, Level state, int frequency_hz, PlayGame display)
    {
        if (frequency_hz < 1)
            throw new ArgumentException($"Frequency must be >=1, but was given {frequency_hz}");
        // Initialize data
        this.State = state;
        this.controller = controller;
        this.display = display;
        cts = new();
        cycleProgress = new Progress<bool>(handler: _ => Update());
        Cycler cycleTimer = new(frequencyHz: frequency_hz, cycleProgress, cts.Token); // Start a loop to call an update every few ms

        scrollStatus = ScrollStatus.Default(state.Player.Location); 
        displayPlan = new(state, scrollStatus);
        lastActions = new(ActionInput.NoAction);
        statusBar = new(margin: 6);
        //ThreadPool.SetMinThreads(workerThreads: 15, completionPortThreads: 0);
        diffs = displayPlan.GetDiffs(displayPlan);
        display.Update(diffs);
    }

    private void Update()
    {
        // ToDo: rethink this logical flow. Should update display first if possible.
        FutureStates futures = new(
                initialPlan: displayPlan,
                initialState: State,
                scrollStatus: scrollStatus,
                mostRecentInputs: lastActions); // plan for possible next states

            displayPlan = new(State, scrollStatus); // save current screen layout
            lastActions = controller.NextAction(State);
            if (lastActions.Where(a => a == ActionInput.Quit).Any())
                return;
            (State, scrollStatus, diffs) = futures.GetFutureDiffs(lastActions);
            display.Update(diffs); // Re-display anything that changed
            statusBar.ShowStatus(State);

        if (State.WinState == WinState.Won)
            statusBar.ShowStatus(State, "Huzzah! You have won!");
        else
            statusBar.ShowStatus(State, "You lost! Try again!");

        cts.Token.ThrowIfCancellationRequested();
    }
}
