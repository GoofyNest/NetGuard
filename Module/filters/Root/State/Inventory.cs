namespace NetGuard.Hot;

// Last CharData snapshot. No incremental updates from other server packets.
public sealed class Inventory
{
    public int Size { get; set; }
    public int ItemCount { get; set; }
    public List<ItemState> Items { get; } = new();
}
