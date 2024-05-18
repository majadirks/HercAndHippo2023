using BlazingHippo.Pages;
using HercAndHippoLibCs;
namespace BlazingHippo;

internal class DisplayLoop
{
    public const int MESSAGE_MARGIN = 3;
    private readonly CancellationTokenSource cts;
    private readonly PlayGame display;
    private readonly GameController controller;
    public Level State { get; private set; }
    private ScrollStatus scrollStatus;
    private DisplayPlan displayPlan;
    private ActionInputPair lastActions;
    private readonly Timer cycleTimer; // Removing this field seems to cause cross-origin request errors ?!
    public DisplayLoop(GameController controller, Level state, int frequency_hz, PlayGame display)
    {
        if (frequency_hz < 1)
            throw new ArgumentException($"Frequency must be >=1, but was given {frequency_hz}");
        // Initialize data
        this.State = state;
        this.controller = controller;
        this.display = display;
        cts = new();
        async void timerCallback(object? _) => await Update();
        cycleTimer = new Timer(callback: timerCallback, state: null, dueTime: 1000 / frequency_hz, period: 1000 / frequency_hz);
        scrollStatus = ScrollStatus.Default(state.Player.Location); 
        displayPlan = new(state, scrollStatus);
        lastActions = new(ActionInput.NoAction);
    }

    private async Task Update()
    {
        lastActions = controller.NextAction(State);
        State = State.RefreshCyclables(lastActions, cts.Token);
        scrollStatus = scrollStatus.Update(State.Player.Location);
        displayPlan = new(State, scrollStatus);
        await display.Update(displayPlan, lastActions);
    }

    public void Stop()
    {
        cts.Cancel();
    }
}
