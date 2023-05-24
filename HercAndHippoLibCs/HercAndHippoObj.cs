using System.Collections;

namespace HercAndHippoLibCs
{
    public abstract record HercAndHippoObj {}

    public class HercAndHippoObjCollection : IEnumerable<HercAndHippoObj>, IEquatable<HercAndHippoObjCollection>
    {
        private readonly HashSet<HercAndHippoObj> wrappedHashSet;
        public HercAndHippoObjCollection(HashSet<HercAndHippoObj> collection) 
        {
            this.wrappedHashSet = collection;
        }
        public HercAndHippoObjCollection AddObject(HercAndHippoObj toAdd)
        {
            HashSet<HercAndHippoObj> withAddition = new(wrappedHashSet) { toAdd };
            return new HercAndHippoObjCollection(withAddition);
        }

        public bool Equals(HercAndHippoObjCollection? other)
           => wrappedHashSet.GetHashCode() == other.wrappedHashSet.GetHashCode() &&
               wrappedHashSet.Zip(other.wrappedHashSet).All(zipped => zipped.First.Equals(zipped.Second));

        public override bool Equals(object? obj) => obj is HercAndHippoObjCollection other && this.Equals(other);

        public override int GetHashCode()
        {
            // Adapted from code by Jon Skeet:
            // https://stackoverflow.com/questions/8094867/good-gethashcode-override-for-list-of-foo-objects-respecting-the-order
            unchecked
            {
                return 19 + wrappedHashSet.Select(takeable => 31 + takeable.GetHashCode()).Sum();
            }
        }

        public IEnumerator<HercAndHippoObj> GetEnumerator() => wrappedHashSet.GetEnumerator();

        public HercAndHippoObjCollection RemoveObject(HercAndHippoObj toRemove)
        {
            HashSet<HercAndHippoObj> removed = new(wrappedHashSet);
            removed.Remove(toRemove);
            return new(removed);
        }

        IEnumerator IEnumerable.GetEnumerator() => wrappedHashSet.GetEnumerator();

    }
}
