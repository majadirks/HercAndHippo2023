﻿using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HercAndHippoLibCs;

/// <summary>
/// Wraps a HashSet, but performs equality check by comparing items in the set rather than
/// by reference equality. This allows two player objects to be equal if they have the same items
/// in their inventory.
/// </summary>
public readonly struct Inventory : IEnumerable<ITakeable>, IEquatable<Inventory>
{
    private readonly HashSet<ITakeable> takeables;
    public static Inventory EmptyInventory { get; } = new();
    public Inventory() => takeables = new HashSet<ITakeable>();
    public Inventory(HashSet<ITakeable> takeables) => this.takeables = takeables;
    public Inventory(IEnumerable<ITakeable> takeables) => this.takeables = new(takeables.ToHashSet());
    public Inventory(ITakeable starterItem) => takeables = new HashSet<ITakeable>() { starterItem };
    public Inventory AddItem(ITakeable item)
    {
        HashSet<ITakeable> newSet = new(takeables) { item };
        return new Inventory(newSet);
    }
    public (bool dropped, ITakeable? item, Inventory newInventoryState) DropItem<T>(Color color)
    {
        ITakeable? item = takeables.Where(item => item.MatchesColor<T>(color)).FirstOrDefault();
        if (item == default) return (false, item, this);
        Inventory newState = this.Where(item => !item.MatchesColor<T>(color)).ToInventory();
        return (true, item, newState);
    }
    public bool Contains<T>(Color color) => takeables.Where(item => item.MatchesColor<T>(color)).Any();
    public IEnumerator<ITakeable> GetEnumerator() => takeables.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => takeables.GetEnumerator();
    public override bool Equals([NotNullWhen(true)] object? obj)
     => obj != null && obj is Inventory other && this.ContainsSameItemsAs(other);
    public bool Equals(Inventory other) => this.ContainsSameItemsAs(other);
    private bool ContainsSameItemsAs(Inventory other)
        => takeables.IsSubsetOf(other.takeables) && other.takeables.IsSubsetOf(takeables);
    public static bool operator ==(Inventory left, Inventory right) => left.Equals(right);  
    public static bool operator !=(Inventory left, Inventory right) => !(left == right);
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 19;
            foreach (var takeable in takeables)
            {
                hash ^= takeable.GetHashCode();
            }
            return hash;
        }
    }
    public int Count => takeables.Count;
}

public static class InventoryExtensions
{
    ///<summary>Returns true if an ITakeable is of the given type and color</summary> 
    public static bool MatchesColor<T>(this ITakeable item, Color color) => item is T && item.Color == color;
    public static Inventory ToInventory(this IEnumerable<ITakeable> enumerable) => new(enumerable);
}