namespace HercAndHippoLibCs;

public static class HashSetExtensions
{
    public static HashSet<T> AddObject<T>(this HashSet<T> collection, T toAdd) => new(collection, collection.Comparer) { toAdd };
    public static HashSet<T> AddObjects<T>(this HashSet<T> collection, params T?[] toAdd) 
        => new(collection.Concat(toAdd.Where(item => item !=null).Cast<T>()), collection.Comparer);
    public static HashSet<T> RemoveObject<T>(this HashSet<T> collection, T toRemove)
    {
        HashSet<T> removed = new(collection, collection.Comparer);
        removed.Remove(toRemove);
        return removed;
    }
}


