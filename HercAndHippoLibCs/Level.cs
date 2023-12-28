using System.Diagnostics.CodeAnalysis;

namespace HercAndHippoLibCs
{
    public class Level
    {
        public Player Player { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public int Gravity { get; init; }
        public int Cycles { get; private set; }
        private HashSet<HercAndHippoObj> SecondaryObjects { get; init; } // secondary, ie not the player
        public Level(Player player, int gravity, HashSet<HercAndHippoObj> secondaryObjects)
        {
            Player = player;
            SecondaryObjects = secondaryObjects;
            Width = GetWidth(secondaryObjects);
            Height = GetHeight(secondaryObjects);
            Gravity = Math.Max(gravity, 0);
            Cycles = 0;
        }
        private Level(Player player, HashSet<HercAndHippoObj> secondaryObjects, int width, int height, int gravity)
        {
            Player = player;
            SecondaryObjects = secondaryObjects;
            Width = width;
            Height = height;
            Gravity = gravity;
            Cycles = 0;
        }
        public HashSet<HercAndHippoObj> LevelObjects => SecondaryObjects.AddObject(Player);
        public Level WithPlayer(Player player) => new (player: player, secondaryObjects: this.SecondaryObjects, width: Width, height: Height, gravity: Gravity);
        public IEnumerable<HercAndHippoObj> ObjectsAt(Location location) => LevelObjects.Where(d => d is ILocatable dAtLoc && dAtLoc.Location.Equals(location));
        public Level Without(HercAndHippoObj toRemove) => new(player: this.Player, secondaryObjects: SecondaryObjects.RemoveObject(toRemove), Width, Height, Gravity);
        public Level AddObject(HercAndHippoObj toAdd) => new(player: this.Player, secondaryObjects: SecondaryObjects.AddObject(toAdd), Width, Height, Gravity);
        public Level Replace(HercAndHippoObj toReplace, HercAndHippoObj toAdd) => this.Without(toReplace).AddObject(toAdd);
        public Level RefreshCyclables(ActionInput actionInput)
        {
            var nextState = LevelObjects // Do not refresh in parallel; this could cause objects to interfere with nearby copies of themselves
            .Where(disp => disp is ICyclable cylable)
            .Cast<ICyclable>()
            .Aggregate(seed: this, func: (state, nextCyclable) => nextCyclable.Cycle(state, actionInput));
            nextState.Cycles = Cycles + 1;
            return nextState;
        }
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

    
