using System.Diagnostics.CodeAnalysis;
namespace HercAndHippoLibCs;

public class Level
{
    public Player Player { get; init; }
    public Hippo? Hippo { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public Gravity Gravity { get; init; }
    public int Cycles { get; private set; }
    public WinState WinState { get; private set; }
    public Level ForceSetCycles(int cycles)
    {
        Cycles = cycles;
        return this;
    }
    private HashSet<HercAndHippoObj> SecondaryObjects { get; init; } // secondary, ie not the player or hippo
    public Level(Player player, Gravity gravity, HashSet<HercAndHippoObj> secondaryObjects, Hippo? hippo = null)
    {
        Player = player;
        Hippo = hippo;
        SecondaryObjects = secondaryObjects;
        Width = GetWidth(secondaryObjects);
        Height = GetHeight(secondaryObjects);
        Gravity = gravity;
        Cycles = 0;
        WinState = WinState.InProgress;
    }
    private Level(Player player, Hippo? hippo, HashSet<HercAndHippoObj> secondaryObjects, int width, int height, int cycles, Gravity gravity, WinState winSate)
    {
        Player = player;
        Hippo = hippo;
        SecondaryObjects = secondaryObjects;
        Width = width;
        Height = height;
        Gravity = gravity;
        Cycles = cycles;
        WinState = winSate;
    }

    public IEnumerable<HercAndHippoObj> LevelObjects => Hippo == null ? SecondaryObjects.Append(Player) : SecondaryObjects.Append(Hippo).Append(Player);
    public Level WithPlayer(Player player) => new (player: player, hippo: Hippo, secondaryObjects: this.SecondaryObjects, width: Width, height: Height, cycles: Cycles, gravity: Gravity, winSate: WinState);
    public IEnumerable<HercAndHippoObj> ObjectsAt(Location location) => LevelObjects.Where(d => d.IsLocatable && ((ILocatable)d).Location.Equals(location));
    public Level Without(HercAndHippoObj toRemove)
    {
        if (toRemove == null)
            throw new ArgumentNullException(nameof(toRemove));
        else if (toRemove is Player)
            throw new NotSupportedException($"Cannot remove player from level using method '{nameof(Without)}'");
        else if (toRemove is Hippo)
            return new Level(player: Player, gravity: Gravity, secondaryObjects: SecondaryObjects, hippo: null, width: Width, height: Height, cycles: Cycles, winSate: WinState);
        else
            return new(player: this.Player, hippo: Hippo, secondaryObjects: SecondaryObjects.RemoveObject(toRemove), Width, Height, Cycles, Gravity, winSate: WinState);
    }
    public Level AddSecondaryObject(HercAndHippoObj toAdd) => new(player: this.Player, hippo: Hippo, secondaryObjects: SecondaryObjects.AddObject(toAdd), Width, Height, Cycles, Gravity, winSate: WinState);
    public Level Replace(HercAndHippoObj toReplace, HercAndHippoObj toAdd)
    {
        if (toReplace == null)
            throw new ArgumentNullException(nameof(toReplace));
        else if (toAdd == null)
            throw new ArgumentNullException(nameof(toAdd));
        else if (toReplace is Player ^ toAdd is Player)
            throw new NotSupportedException();
        else if (toReplace is Hippo ^ toAdd is Hippo)
            throw new NotSupportedException();
        else if (toAdd is Player newPlayer) // from above logic, toReplace must also be a player
            return new Level(player: newPlayer, hippo: Hippo, gravity: Gravity, secondaryObjects: SecondaryObjects, width: Width, height: Height, cycles: Cycles, winSate: WinState);
        else if (toAdd is Hippo newHippo) // from aboveLogic, toReplace must also be a hippo
            return new(player: Player, hippo: newHippo, gravity: Gravity, secondaryObjects: SecondaryObjects, cycles: Cycles, height: Height, width: Width, winSate: WinState);
        else
        {
            var updatedSo = SecondaryObjects.Where(obj => obj != toReplace).Append(toAdd).ToHashSet();
            return new(player: Player, hippo: Hippo, gravity: Gravity, secondaryObjects: updatedSo, width: Width, height: Height, cycles: Cycles, winSate: WinState);
        }
                 
    }
    public Level RefreshCyclables(ActionInputPair actionInputs, CancellationToken? cancellationToken = null)
    {
        CancellationToken token = cancellationToken ?? CancellationToken.None;
        Level nextState = this;
        bool comboAction = actionInputs.IsComboAction;
        // First cycle player using first input
        nextState = nextState.Player.Cycle(nextState, actionInputs.First);
        // Then cycle player again using the second input
        if (comboAction)
            nextState = nextState.Player.Cycle(nextState, actionInputs.Second);
        // Then cycle hippo if relevant
        if (nextState.Hippo != null)
            nextState = nextState.Hippo.Cycle(nextState, actionInputs.First);
        // Cycle a second time if locked to player
        if (nextState.Hippo != null && nextState.Hippo.LockedToPlayer && comboAction)
            nextState = nextState.Hippo.Cycle(nextState, actionInputs.Second);
        // Then cycle non-player objects
        nextState = SecondaryObjects // Do not refresh in parallel; this could cause objects to interfere with nearby copies of themselves, and can make updating slower
            .Where(disp => disp.IsCyclable)
            .Cast<ICyclable>()
            .TakeWhile(dummy => !token.IsCancellationRequested)
            .Aggregate(
            seed: nextState, 
            func: (state, nextCyclable) => nextCyclable.Cycle(state, comboAction ? actionInputs.Second : actionInputs.First));

        // Finally, if hippo is locked to player, hippo should move in response to any player motion
        Hippo? hippo = nextState.Hippo;
        if (hippo != null && hippo.LockedToPlayer)
        {
            nextState = Hippo.LockAbovePlayer(nextState);
        }
        nextState.Cycles = Cycles + 1;
        return nextState;
    }
    private bool HasSameStateAs(Level otherState)
        => SecondaryObjects.Count == otherState.SecondaryObjects.Count &&
           LevelObjects.Zip(otherState.LevelObjects).All(zipped => zipped.First.Equals(zipped.Second));
    public bool Contains(HercAndHippoObj obj) => LevelObjects.Contains(obj);
    public bool GravityApplies() => HasGravity && Cycles > 0 && Cycles % Gravity.WaitCycles == 0;
    public bool HasGravity => Gravity.Strength > 0;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Level other && other.HasSameStateAs(this);
    public static bool operator ==(Level left, Level right) => left.Equals(right);
    public static bool operator !=(Level left, Level right) => !(left == right);
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = Player.GetHashCode() ^ Gravity.GetHashCode();
            if (Hippo != null) hash &= Hippo.GetHashCode();
            foreach (var hho in SecondaryObjects)
                hash ^= hho.GetHashCode();
            return hash;
        }
    }
    public override string ToString()
    {
        string hippoStr = Hippo == null ? "" : $" and Hippo at {Hippo.Location}, " + (Hippo.LockedToPlayer ? "locked to player." : "not locked to player");
        string gravityStr = GravityApplies() ? "Gravity applies." : "Gravity does not apply.";
        string desc = $"Level with Player at {Player.Location}{hippoStr}; Object count = {SecondaryObjects.Count}, Cycles = {Cycles}. {gravityStr}.";
        return desc;
    }
    private static int GetWidth(HashSet<HercAndHippoObj> ds) => ds.Where(ds => ds.IsLocatable).Cast<ILocatable>().Select(d => (int)d.Location.Col).DefaultIfEmpty(0).Max();
    private static int GetHeight(HashSet<HercAndHippoObj> ds) => ds.Where(ds => ds.IsLocatable).Cast<ILocatable>().Select(d => (int)d.Location.Row).DefaultIfEmpty(0).Max();
    public Message? GetMessage() 
        => (Message?)SecondaryObjects
        .Where(obj => obj is Message m && m.RemainingCycles > 0)
        .LastOrDefault();

    public Level Win()
    {
        WinState = WinState.Won;
        return this;
    }

    public Level Lose()
    {
        WinState = WinState.Lost;
        return this;
    }

}
