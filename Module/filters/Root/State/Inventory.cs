namespace NetGuard.Hot;

public sealed class Inventory
{
    public int Size { get; set; }
    public int ItemCount { get; set; }
    public List<ItemState> Items { get; } = new();
    public bool IsSynchronized { get; set; } = true;
    public string LastError { get; set; } = "";
    public string LastSyncLogKey { get; set; } = "";
    public object Sync { get; } = new();
    // Separate slot spaces, needed to identify items returning from storage/pets/buyback.
    public Dictionary<string, Inventory> Containers { get; } = new();
    public Dictionary<string, List<byte>> PendingSnapshots { get; } = new();
    public Dictionary<uint, string> NpcCodes { get; } = new();

    public ItemState? GetItem(int slot) => Items.FirstOrDefault(i => i.Slot == slot);
    public void Remove(int slot)
    {
        Items.RemoveAll(i => i.Slot == slot);
        ItemCount = Items.Count;
    }
    public void Put(ItemState item)
    {
        if (item.Slot < 0 || item.Slot >= 254) throw new InvalidDataException("Invalid inventory slot.");
        Remove(item.Slot);
        if (item.Amount > 0 || !item.AmountKnown) Items.Add(item);
        Items.Sort((a, b) => a.Slot.CompareTo(b.Slot));
        ItemCount = Items.Count;
        if (!item.IsKnown || !item.AmountKnown) Invalidate($"Item details unknown at slot {item.Slot}.");
    }
    public void Invalidate(string reason) { IsSynchronized = false; LastError = reason; }
    public Inventory Container(string key)
    {
        if (!Containers.TryGetValue(key, out var container)) Containers[key] = container = new Inventory();
        return container;
    }
    public Inventory Copy()
    {
        var copy = new Inventory { Size = Size, ItemCount = ItemCount, IsSynchronized = IsSynchronized, LastError = LastError, LastSyncLogKey = LastSyncLogKey };
        copy.Items.AddRange(Items.Select(i => i.Copy()));
        foreach (var pair in Containers) copy.Containers[pair.Key] = pair.Value.Copy();
        foreach (var pair in PendingSnapshots) copy.PendingSnapshots[pair.Key] = new List<byte>(pair.Value);
        foreach (var pair in NpcCodes) copy.NpcCodes[pair.Key] = pair.Value;
        return copy;
    }
    public void Commit(Inventory copy)
    {
        Size = copy.Size; IsSynchronized = copy.IsSynchronized; LastError = copy.LastError;
        LastSyncLogKey = copy.LastSyncLogKey;
        Items.Clear(); Items.AddRange(copy.Items); ItemCount = Items.Count;
        Containers.Clear(); foreach (var pair in copy.Containers) Containers.Add(pair.Key, pair.Value);
        PendingSnapshots.Clear(); foreach (var pair in copy.PendingSnapshots) PendingSnapshots.Add(pair.Key, pair.Value);
        NpcCodes.Clear(); foreach (var pair in copy.NpcCodes) NpcCodes.Add(pair.Key, pair.Value);
    }
}
