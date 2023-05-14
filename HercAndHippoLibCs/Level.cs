using System.Diagnostics.CodeAnalysis;

namespace HercAndHippoLibCs
{
    public readonly struct Level
    {
        public Player Player { get; private init; }
        public readonly int Width { get; private init; }
        public readonly int Height { get; private init; }
        private HashSet<IDisplayable> Displayables { get; init; }
        public Level(Player player, HashSet<IDisplayable> displayables)
            => (Player, Displayables, Width, Height) = (player, displayables, GetWidth(displayables), GetHeight(displayables));
        public HashSet<IDisplayable> LevelObjects => Displayables.AddObject(Player);
        public Level WithPlayer(Player player) => this with { Player = player };
        public IEnumerable<IDisplayable> ObjectsAt(Location location) => LevelObjects.Where(d => d.Location.Equals(location));
        public Level Without(IDisplayable toRemove) => this with { Displayables = Displayables.RemoveObject(toRemove) };
        public Level AddObject(IDisplayable toAdd) => this with { Displayables = Displayables.AddObject(toAdd) };
        public Level Replace(IDisplayable toReplace, IDisplayable toAdd) => this.Without(toReplace).AddObject(toAdd);
        public Level RefreshCyclables(ActionInput actionInput)
            => LevelObjects
            .AsParallel()
            .Where(disp => disp is ICyclable cylable)
            .Cast<ICyclable>()     
            .Aggregate(seed: this, func: (state, nextCyclable) => nextCyclable.Cycle(state, actionInput));
        private bool HasSameStateAs(Level otherState)
            => LevelObjects.Count == otherState.LevelObjects.Count &&
               LevelObjects.Zip(otherState.LevelObjects).All(zipped => zipped.First.Equals(zipped.Second));
        public bool Contains(IDisplayable obj) => LevelObjects.Contains(obj);
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Level other && other.HasSameStateAs(this);
        public static bool operator ==(Level left, Level right) => left.Equals(right);
        public static bool operator !=(Level left, Level right) => !(left == right);
        public override int GetHashCode() => LevelObjects.GetHashCode();
        public override string ToString() => $"Level with Player at {Player.Location}; Object count = {Displayables.Count}.";
        private static int GetWidth(HashSet<IDisplayable> ds) => ds.Select(d => d.Location.Col).Max() ?? 0;
        private static int GetHeight(HashSet<IDisplayable> ds) => ds.Select(d => d.Location.Row).Max() ?? 0;
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

    
