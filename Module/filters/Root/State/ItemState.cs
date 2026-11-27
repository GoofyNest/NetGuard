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
    public int Amount { get; set; } = 1;
    public byte State { get; set; }
}
