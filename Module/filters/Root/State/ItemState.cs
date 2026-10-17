namespace NetGuard.Hot;

public sealed class ItemState
{
    public int Slot { get; set; }
    public long Id { get; set; }
    public string CodeName128 { get; set; } = "";
    public int OptLevel { get; set; }
    public ulong Variance { get; set; }
    public long Durability { get; set; }
    public int MagParamNum { get; set; }
}