namespace NetGuard.Hot;

public sealed class CharState
{
    public string Charname { get; set; } = "";
    public bool IsVisible { get; set; }
    public long ServerTime { get; set; }
    public long RefObjID { get; set; }
    public int Scale { get; set; }
    public int CurLevel { get; set; }
    public int MaxLevel { get; set; }
    public ulong ExpOffset { get; set; }
    public long SExpOffset { get; set; }
    public ulong RemainGold { get; set; }
    public long RemainSkillPoint { get; set; }
    public int RemainStatPoint { get; set; }
    public int RemainHwanCount { get; set; }
    public long GatheredExpPoint { get; set; }
    public long Hp { get; set; }
    public long Mp { get; set; }
    public int AutoInverstExp { get; set; }
    public int DailyPk { get; set; }
    public int TotalPk { get; set; }
    public long PkPenaltyPoint { get; set; }
    public int HwanLevel { get; set; }
    public int PvpCape { get; set; }

    public Inventory Inventory { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}