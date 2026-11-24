using System;
using System.Collections.Generic;
using System.Text;

namespace Module.Helpers.ClientData
{
    // Extended client text layout. Reader also supports the 104-column SQL export.
    public enum CharField
    {
        Service,
        ID,
        CodeName128,
        ObjName128,
        OrgObjCodeName128,
        NameStrID128,
        DescStrID128,
        CashItem,
        Bionic,
        TypeID1,
        TypeID2,
        TypeID3,
        TypeID4,
        DecayTime,
        Country,
        Rarity,
        CanTrade,
        CanSell,
        CanBuy,
        CanBorrow,
        CanDrop,
        CanPick,
        CanRepair,
        CanRevive,
        CanUse,
        CanThrow,
        Price,
        CostRepair,
        CostRevive,
        CostBorrow,
        KeepingFee,
        SellPrice,
        ReqLevelType1,
        ReqLevel1,
        ReqLevelType2,
        ReqLevel2,
        ReqLevelType3,
        ReqLevel3,
        ReqLevelType4,
        ReqLevel4,
        MaxContain,
        RegionID,
        Dir,
        OffsetX,
        OffsetY,
        OffsetZ,
        Speed1,
        Speed2,
        Scale,
        BCHeight,
        BCRadius,
        EventID,
        AssocFileObj128,
        AssocFileDrop128,
        AssocFileIcon128,
        AssocFile1_128,
        AssocFile2_128,
        Lvl,
        CharGender,
        MaxHP,
        MaxMP,
        Resistance1,
        Resistance2,
        Resistance3,
        Resistance4,
        Resistance5,
        Resistance6,
        InventorySize,
        CanStore_TID1,
        CanStore_TID2,
        CanStore_TID3,
        CanStore_TID4,
        CanBeVehicle,
        CanControl,
        DamagePortion,
        MaxPassenger,
        AssocTactics,
        PD,
        MD,
        PAR,
        MAR,
        ER,
        BR,
        HR,
        CHR,
        ExpToGive,
        CreepType,
        Knockdown,
        KO_RecoverTime,
        DefaultSkill_1,
        DefaultSkill_2,
        DefaultSkill_3,
        DefaultSkill_4,
        DefaultSkill_5,
        DefaultSkill_6,
        DefaultSkill_7,
        DefaultSkill_8,
        DefaultSkill_9,
        DefaultSkill_10,
        TextureType,
        Except_1,
        Except_2,
        Except_3,
        Except_4,
        Except_5,
        Except_6,
        Except_7,
        Except_8,
        Except_9,
        Except_10,
    }

    public sealed class CharData
    {
        public int Service { get; set; }
        public int ID { get; set; }
        public string CodeName128 { get; set; } = string.Empty;
        public string ObjName128 { get; set; } = string.Empty;
        public string OrgObjCodeName128 { get; set; } = string.Empty;
        public string NameStrID128 { get; set; } = string.Empty;
        public string DescStrID128 { get; set; } = string.Empty;
        public int CashItem { get; set; }
        public int Bionic { get; set; }
        public int TypeID1 { get; set; }
        public int TypeID2 { get; set; }
        public int TypeID3 { get; set; }
        public int TypeID4 { get; set; }
        public int DecayTime { get; set; }
        public int Country { get; set; }
        public int Rarity { get; set; }
        public int CanTrade { get; set; }
        public int CanSell { get; set; }
        public int CanBuy { get; set; }
        public int CanBorrow { get; set; }
        public int CanDrop { get; set; }
        public int CanPick { get; set; }
        public int CanRepair { get; set; }
        public int CanRevive { get; set; }
        public int CanUse { get; set; }
        public int CanThrow { get; set; }
        public int Price { get; set; }
        public int CostRepair { get; set; }
        public int CostRevive { get; set; }
        public int CostBorrow { get; set; }
        public int KeepingFee { get; set; }
        public int SellPrice { get; set; }
        public int ReqLevelType1 { get; set; }
        public int ReqLevel1 { get; set; }
        public int ReqLevelType2 { get; set; }
        public int ReqLevel2 { get; set; }
        public int ReqLevelType3 { get; set; }
        public int ReqLevel3 { get; set; }
        public int ReqLevelType4 { get; set; }
        public int ReqLevel4 { get; set; }
        public int MaxContain { get; set; }
        public int RegionID { get; set; }
        public int Dir { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetZ { get; set; }
        public float Speed1 { get; set; }
        public float Speed2 { get; set; }
        public int Scale { get; set; }
        public float BCHeight { get; set; }
        public float BCRadius { get; set; }
        public int EventID { get; set; }
        public string AssocFileObj128 { get; set; } = string.Empty;
        public string AssocFileDrop128 { get; set; } = string.Empty;
        public string AssocFileIcon128 { get; set; } = string.Empty;
        public string AssocFile1_128 { get; set; } = string.Empty;
        public string AssocFile2_128 { get; set; } = string.Empty;
        public int Lvl { get; set; }
        public int CharGender { get; set; }
        public int MaxHP { get; set; }
        public int MaxMP { get; set; }
        // Keep numeric labels until this client's resistance names are verified.
        public int Resistance1 { get; set; }
        public int Resistance2 { get; set; }
        public int Resistance3 { get; set; }
        public int Resistance4 { get; set; }
        public int Resistance5 { get; set; }
        public int Resistance6 { get; set; }
        // Client-specific trailing columns are preserved, not assigned guessed meanings.
        public string[] ExtraColumns { get; set; } = Array.Empty<string>();
        public int InventorySize { get; set; }
        public int CanStore_TID1 { get; set; }
        public int CanStore_TID2 { get; set; }
        public int CanStore_TID3 { get; set; }
        public int CanStore_TID4 { get; set; }
        public int CanBeVehicle { get; set; }
        public int CanControl { get; set; }
        public int DamagePortion { get; set; }
        public int MaxPassenger { get; set; }
        public int AssocTactics { get; set; }
        public float PD { get; set; }
        public float MD { get; set; }
        public int PAR { get; set; }
        public int MAR { get; set; }
        public float ER { get; set; }
        public int BR { get; set; }
        public float HR { get; set; }
        public int CHR { get; set; }
        public long ExpToGive { get; set; }
        public int CreepType { get; set; }
        public int Knockdown { get; set; }
        public int KO_RecoverTime { get; set; }
        public int DefaultSkill_1 { get; set; }
        public int DefaultSkill_2 { get; set; }
        public int DefaultSkill_3 { get; set; }
        public int DefaultSkill_4 { get; set; }
        public int DefaultSkill_5 { get; set; }
        public int DefaultSkill_6 { get; set; }
        public int DefaultSkill_7 { get; set; }
        public int DefaultSkill_8 { get; set; }
        public int DefaultSkill_9 { get; set; }
        public int DefaultSkill_10 { get; set; }
        public int TextureType { get; set; }
        public double Except_1 { get; set; }
        public double Except_2 { get; set; }
        public double Except_3 { get; set; }
        public double Except_4 { get; set; }
        public double Except_5 { get; set; }
        public double Except_6 { get; set; }
        public double Except_7 { get; set; }
        public double Except_8 { get; set; }
        public double Except_9 { get; set; }
        public double Except_10 { get; set; }

        public override string ToString() => string.Join(
            Environment.NewLine,
            GetType().GetProperties().Select(property =>
                $"{property.Name}: {(property.GetValue(this) is string[] values ? string.Join(", ", values) : property.GetValue(this))}"));
    }
}
