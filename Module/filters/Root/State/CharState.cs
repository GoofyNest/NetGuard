namespace NetGuard.Hot;

public sealed class CharState
{
    public DateTime ScudDispelBlockedUntil { get; set; } = DateTime.MinValue;
    public uint UniqueID { get; set; }
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

    public ushort LastUseItem { get; set; }

    public List<CosState> Pets { get; set; } = new();
    public object PetSync { get; } = new();
    public bool PetStateReady { get; set; }
    public bool AttackPetPending { get; set; }
    public bool GrabPetPending { get; set; }
    // One entry per forwarded 0x704C, including non-pet uses (type 0).
    public Queue<int> PendingItemUses { get; set; } = new();

    public Inventory Inventory { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public sealed class CosState
{

    public uint UniqueID { get; set; }
    public uint ObjectID { get; set; }

    public int Type { get; set; } /* 1 = attack, 2 = grab pet */

    public bool Summoned { get; set; } = true;
}
