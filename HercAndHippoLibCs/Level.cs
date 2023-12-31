using System.Diagnostics.CodeAnalysis;

namespace HercAndHippoLibCs
{
    public class Level
    {
        public Player Player { get; init; }
        public Hippo? Hippo { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public Gravity Gravity { get; init; }
        public int Cycles { get; private set; }
        public void ForceSetCycles(int cycles) => Cycles = cycles;
        private HashSet<HercAndHippoObj> SecondaryObjects { get; init; } // secondary, ie not the player
        public Level(Player player, Gravity gravity, HashSet<HercAndHippoObj> secondaryObjects, Hippo? hippo = null)
        {
            Player = player;
            Hippo = hippo;
            SecondaryObjects = secondaryObjects;
            Width = GetWidth(secondaryObjects);
            Height = GetHeight(secondaryObjects);
            Gravity = gravity;
            Cycles = 0;
        }
        private Level(Player player, Hippo? hippo, HashSet<HercAndHippoObj> secondaryObjects, int width, int height, int cycles, Gravity gravity)
        {
            Player = player;
            Hippo = hippo;
            SecondaryObjects = secondaryObjects;
            Width = width;
            Height = height;
            Gravity = gravity;
            Cycles = cycles;
        }

        /// <summary>
        /// Return a HashSet containing the player and all secondary objects. 
        /// Somewhat slow; probably better to act on player and secondary objects separately 
        /// when performance is critical.
        /// </summary>
        public HashSet<HercAndHippoObj> LevelObjects => SecondaryObjects.AddObject(Player);
        public Level WithPlayer(Player player) => new (player: player, hippo: Hippo, secondaryObjects: this.SecondaryObjects, width: Width, height: Height, cycles: Cycles, gravity: Gravity);
        public IEnumerable<HercAndHippoObj> ObjectsAt(Location location) => LevelObjects.Where(d => d is ILocatable dAtLoc && dAtLoc.Location.Equals(location));
        public Level Without(HercAndHippoObj toRemove) => new(player: this.Player, hippo: Hippo, secondaryObjects: SecondaryObjects.RemoveObject(toRemove), Width, Height, Cycles, Gravity);
        public Level AddObject(HercAndHippoObj toAdd) => new(player: this.Player, hippo: Hippo, secondaryObjects: SecondaryObjects.AddObject(toAdd), Width, Height, Cycles, Gravity);
        public Level Replace(HercAndHippoObj toReplace, HercAndHippoObj toAdd) => this.Without(toReplace).AddObject(toAdd);
        public Level RefreshCyclables(ActionInput actionInput, CancellationToken? cancellationToken = null)
        {
            CancellationToken token = cancellationToken ?? CancellationToken.None;
            // First cycle non-player objects
            var nextState = SecondaryObjects // Do not refresh in parallel; this could cause objects to interfere with nearby copies of themselves, and can make updating slower
                .Where(disp => disp.IsCyclable)
                .Cast<ICyclable>()
                .TakeWhile(dummy => !token.IsCancellationRequested)
                .Aggregate(
                seed: this, 
                func: (state, nextCyclable) => nextCyclable.Cycle(state, actionInput));
            // Then cycle player
            nextState = nextState.Player.Cycle(nextState, actionInput);
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
        public override int GetHashCode() => LevelObjects.GetHashCode();
        public override string ToString() => $"Level with Player at {Player.Location}; Object count = {SecondaryObjects.Count}.";
        private static int GetWidth(HashSet<HercAndHippoObj> ds) => ds.Where(ds => ds is ILocatable d).Cast<ILocatable>().Select(d => d.Location.Col).Max() ?? 0;
        private static int GetHeight(HashSet<HercAndHippoObj> ds) => ds.Where(ds => ds is ILocatable d).Cast<ILocatable>().Select(d => d.Location.Row).Max() ?? 0;
        public bool TryGetHippo(out Hippo? hippo)
        {
            hippo = Hippo;
            return hippo != null;
        }
    }

    public static class HashSetExtensions
    {
        public static HashSet<T> AddObject<T>(this HashSet<T> collection, T toAdd) => new(collection, collection.Comparer) { toAdd };
        public static HashSet<T> RemoveObject<T>(this HashSet<T> collection, T toRemove)
        {
            HashSet<T> removed = new(collection, collection.Comparer);
            removed.Remove(toRemove);
            return removed;
        }
    }
}

    
