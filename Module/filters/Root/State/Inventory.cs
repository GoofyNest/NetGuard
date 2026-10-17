namespace NetGuard.Hot;

public sealed class Inventory
{
    public int Size { get; set; }
    public int ItemCount { get; set; }
    public List<ItemState> Items { get; } = new();
}