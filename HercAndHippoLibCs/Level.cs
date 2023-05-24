using System.Diagnostics.CodeAnalysis;

namespace HercAndHippoLibCs
{
    public readonly struct Level
    {
        public Player Player { get; private init; }
        public readonly int Width { get; private init; }
        public readonly int Height { get; private init; }
        private HashSet<HercAndHippoObj> SecondaryObjects { get; init; } // secondary, ie not the player
        public Level(Player player, HashSet<HercAndHippoObj> displayables)
            => (Player, SecondaryObjects, Width, Height) = (player, displayables, GetWidth(displayables), GetHeight(displayables));
        public HashSet<HercAndHippoObj> LevelObjects => SecondaryObjects.AddObject(Player);
        public Level WithPlayer(Player player) => this with { Player = player };
        public IEnumerable<HercAndHippoObj> ObjectsAt(Location location) => LevelObjects.Where(d => d is ILocatable dAtLoc && dAtLoc.Location.Equals(location));
        public Level Without(HercAndHippoObj toRemove) => this with { SecondaryObjects = SecondaryObjects.RemoveObject(toRemove) };
        public Level AddObject(HercAndHippoObj toAdd) => this with { SecondaryObjects = SecondaryObjects.AddObject(toAdd) };
        public Level Replace(HercAndHippoObj toReplace, HercAndHippoObj toAdd) => this.Without(toReplace).AddObject(toAdd);
        public Level RefreshCyclables(ActionInput actionInput)
            => LevelObjects // Do not refresh in parallel; this could cause objects to interfere with nearby copies of themselves
            .Where(disp => disp is ICyclable cylable)
            .Cast<ICyclable>()     
            .Aggregate(seed: this, func: (state, nextCyclable) => nextCyclable.Cycle(state, actionInput));
        private bool HasSameStateAs(Level otherState)
            => SecondaryObjects.Count == otherState.SecondaryObjects.Count &&
               LevelObjects.Zip(otherState.LevelObjects).All(zipped => zipped.First.Equals(zipped.Second));
        public bool Contains(HercAndHippoObj obj) => LevelObjects.Contains(obj);
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Level other && other.HasSameStateAs(this);
        public static bool operator ==(Level left, Level right) => left.Equals(right);
        public static bool operator !=(Level left, Level right) => !(left == right);
        public override int GetHashCode() => LevelObjects.GetHashCode();
        public override string ToString() => $"Level with Player at {Player.Location}; Object count = {SecondaryObjects.Count}.";
        private static int GetWidth(HashSet<HercAndHippoObj> ds) => ds.Where(ds => ds is ILocatable d).Cast<ILocatable>().Select(d => d.Location.Col).Max() ?? 0;
        private static int GetHeight(HashSet<HercAndHippoObj> ds) => ds.Where(ds => ds is ILocatable d).Cast<ILocatable>().Select(d => d.Location.Row).Max() ?? 0;
    }

    public static class HashSetExtensions
    {
        public static HashSet<T> AddObject<T>(this HashSet<T> collection, T toAdd) => new(collection) { toAdd };
        public static HashSet<T> RemoveObject<T>(this HashSet<T> collection, T toRemove)
        {
            HashSet<T> removed = new(collection);
            removed.Remove(toRemove);
            return removed;
        }
    }
}

    
