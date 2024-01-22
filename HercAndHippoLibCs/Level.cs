namespace HercAndHippoLibCs;

public record Level(Player Player, Hippo? Hippo, HashSet<HercAndHippoObj> SecondaryObjects, int Width, int Height, int Cycles, Gravity Gravity, WinState WinState)
{
    public Level ForceSetCycles(int cycles)
    => this with { Cycles = cycles };
    
    public Level(Player player, Gravity gravity, HashSet<HercAndHippoObj> secondaryObjects, Hippo? hippo = null)
    :this(
        Player: player,
        Hippo: hippo,
        SecondaryObjects: secondaryObjects,
        Width: GetWidth(secondaryObjects),
        Height: GetHeight(secondaryObjects),
        Gravity: gravity,
        Cycles: 0,
        WinState: WinState.InProgress)
    { }

    public IEnumerable<HercAndHippoObj> LevelObjects => Hippo == null ? SecondaryObjects.Append(Player) : SecondaryObjects.Append(Hippo).Append(Player);
    public Level WithPlayer(Player player) => new (Player: player, Hippo: Hippo, SecondaryObjects: this.SecondaryObjects, Width: Width, Height: Height, Cycles: Cycles, Gravity: Gravity, WinState: WinState);
    public IEnumerable<HercAndHippoObj> ObjectsAt(Location location) => LevelObjects.Where(d => d.IsLocatable && ((ILocatable)d).Location.Equals(location));
    public Level Without(HercAndHippoObj toRemove)
    {
        if (toRemove == null)
            throw new ArgumentNullException(nameof(toRemove));
        else if (toRemove is Player)
            throw new NotSupportedException($"Cannot remove player from level using method '{nameof(Without)}'");
        else if (toRemove is Hippo)
            return new Level(Player: Player, Gravity: Gravity, SecondaryObjects: SecondaryObjects, Hippo: null, Width: Width, Height: Height, Cycles: Cycles, WinState: WinState);
        else
            return this with { SecondaryObjects = SecondaryObjects.RemoveObject(toRemove) };
    }
    public Level AddSecondaryObject(HercAndHippoObj toAdd) => new(Player: this.Player, Hippo: Hippo, SecondaryObjects: SecondaryObjects.AddObject(toAdd), Width, Height, Cycles, Gravity, WinState: WinState);
    public Level ReplaceIfPresent(HercAndHippoObj toReplace, HercAndHippoObj toAdd)
    {
        if (toReplace == null)
            throw new ArgumentNullException(nameof(toReplace));
        else if (toAdd == null)
            throw new ArgumentNullException(nameof(toAdd));
        else if (toReplace is Player ^ toAdd is Player)
            throw new NotSupportedException();
        else if (toReplace is Hippo ^ toAdd is Hippo)
            throw new NotSupportedException();
        // If object was removed, do not replace it
        else if (!LevelObjects.Contains(toReplace))
            return this;
        else if (toAdd is Player newPlayer) // from above logic, toReplace must also be a player
            return this with { Player = newPlayer };
        else if (toAdd is Hippo newHippo) // from aboveLogic, toReplace must also be a hippo
            return this with { Hippo = newHippo };
        else
        {
            var updatedSo = SecondaryObjects.Where(obj => obj != toReplace).Append(toAdd).ToHashSet();
            return this with { SecondaryObjects = updatedSo };
        }    
    }
    public Level RefreshCyclables(ActionInputPair actionInputs, CancellationToken? cancellationToken = null)
    {
        CancellationToken token = cancellationToken ?? CancellationToken.None;
        Level nextState = this;
        bool comboAction = actionInputs.IsComboAction;

        // First cycle non-player objects
        nextState = SecondaryObjects // Do not refresh in parallel; this could cause objects to interfere with nearby copies of themselves, and can make updating slower
            .Where(disp => disp.IsCyclable)
            .Cast<ICyclable>()
            .TakeWhile(_ => !token.IsCancellationRequested)
            .Aggregate(
            seed: nextState, 
            func: (state, nextCyclable) => nextCyclable.Cycle(state, comboAction ? actionInputs.Second : actionInputs.First));
        // Then cycle player using first input
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

        // Finally, if hippo is locked to player, hippo should move in response to any player motion
        Hippo? hippo = nextState.Hippo;
        if (hippo != null && hippo.LockedToPlayer)
        {
            nextState = Hippo.LockAbovePlayer(nextState);
        }
        return nextState with { Cycles = Cycles + 1 };
    }
    public bool HasSameStateAs(Level otherState)
    {
        bool hipposNull = Hippo == null && otherState.Hippo == null;
        bool hipposEqual = hipposNull || Hippo != null && Hippo.Equals(otherState.Hippo);
        return SecondaryObjects.Count == otherState.SecondaryObjects.Count &&
           LevelObjects.Zip(otherState.LevelObjects).All(zipped => zipped.First.Equals(zipped.Second)) &&
           Player.Equals(otherState.Player) && hipposEqual;
    }
    public bool Contains(HercAndHippoObj obj) => LevelObjects.Contains(obj);
    public bool GravityApplies() => HasGravity && Cycles > 0 && Cycles % Gravity.WaitCycles == 0;
    public bool HasGravity => Gravity.Strength > 0;
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

    public Level Win() => this with { WinState = WinState.Won };

    public Level Lose() => this with { WinState = WinState.Lost };

}
