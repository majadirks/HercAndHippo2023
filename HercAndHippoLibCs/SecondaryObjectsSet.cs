using System.Collections;
namespace HercAndHippoLibCs;

public class SecondaryObjectsSet : IEnumerable, IEnumerable<HercAndHippoObj>
{
    private readonly List<HercAndHippoObj> objs;
    public int Count => objs.Count;
    public SecondaryObjectsSet(IEnumerable<HercAndHippoObj> objs)
    {
        this.objs = objs.ToList();
    }

    public SecondaryObjectsSet()
    {
        this.objs = new();
    }

    public SecondaryObjectsSet RemoveObject(HercAndHippoObj toRemove)
    {
        var newobjs = objs.Where(obj => obj != toRemove).ToList();
        return new(newobjs);
    }

    /// <summary>
    /// Add method used by collection initializers
    /// </summary>
    public void Add(HercAndHippoObj toAdd)
    {
        if (toAdd is Hippo || toAdd is Player)
            throw new InvalidOperationException($"Object {toAdd} is not a secondary object.");
        objs.Add(toAdd);
    }

    /// <summary>
    /// Creates a copy of this set that includes the object to be added
    /// </summary>
    public SecondaryObjectsSet AddObject(HercAndHippoObj toAdd)
    {
        if (toAdd is Hippo || toAdd is Player)
            throw new InvalidOperationException($"Object {toAdd} is not a secondary object.");
        var newobjs = objs.Append(toAdd).ToList();
        return new(newobjs);
    }

    public SecondaryObjectsSet Replace(HercAndHippoObj toRemove, HercAndHippoObj toAdd)
    {
        if (toAdd is Hippo || toAdd is Player)
            throw new InvalidOperationException($"Object {toAdd} is not a secondary object.");
        var newobjs = objs.Where(obj =>obj != toRemove).Append(toAdd).ToList();
        return new(newobjs);
    }

    public SecondaryObjectsSet ReplaceMany(SecondaryObjectsSet toRemove, SecondaryObjectsSet toAdd)
    {
        HashSet<HercAndHippoObj> toRemoveHs = toRemove.ToHashSet();
        SecondaryObjectsSet newSos = new(objs.Where(obj => !toRemoveHs.Contains(obj)).Concat(toAdd));
        return new(newSos);
    }

    public void Clear() => objs.Clear();

    public int GetWidth() 
        => objs.Where(ds => ds.IsLocatable).Cast<ILocatable>().Select(d => (int)d.Location.Col + 1).Max();
    public int GetHeight() => 
        objs.Where(ds => ds.IsLocatable).Cast<ILocatable>().Select(d => (int)d.Location.Row + 1).Max();

    public IEnumerator<HercAndHippoObj> GetEnumerator() 
        => ((IEnumerable<HercAndHippoObj>)objs).GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)objs).GetEnumerator();  
}
