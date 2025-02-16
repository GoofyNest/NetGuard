namespace Module.Helpers.ClientData
{
    public enum ItemField
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
        MaxStack,
        ReqGender,
        ReqStr,
        ReqInt,
        ItemClass,
        SetID,
        Dur_L,
        Dur_U,
        PD_L,
        PD_U,
        PDInc,
        ER_L,
        ER_U,
        ERInc,
        PAR_L,
        PAR_U,
        PARInc,
        BR_L,
        BR_U,
        MD_L,
        MD_U,
        MDInc,
        MAR_L,
        MAR_U,
        MARInc,
        PDStr_L,
        PDStr_U,
        MDInt_L,
        MDInt_U,
        Quivered,
        Ammo1_TID4,
        Ammo2_TID4,
        Ammo3_TID4,
        Ammo4_TID4,
        Ammo5_TID4,
        SpeedClass,
        TwoHanded,
        Range,
        PAttackMin_L,
        PAttackMin_U,
        PAttackMax_L,
        PAttackMax_U,
        PAttackInc,
        MAttackMin_L,
        MAttackMin_U,
        MAttackMax_L,
        MAttackMax_U,
        MAttackInc,
        PAStrMin_L,
        PAStrMin_U,
        PAStrMax_L,
        PAStrMax_U,
        MAInt_Min_L,
        MAInt_Min_U,
        MAInt_Max_L,
        MAInt_Max_U,
        HR_L,
        HR_U,
        HRInc,
        CHR_L,
        CHR_U,
        Param1,
        Desc1_128,
        Param2,
        Desc2_128,
        Param3,
        Desc3_128,
        Param4,
        Desc4_128,
        Param5,
        Desc5_128,
        Param6,
        Desc6_128,
        Param7,
        Desc7_128,
        Param8,
        Desc8_128,
        Param9,
        Desc9_128,
        Param10,
        Desc10_128,
        Param11,
        Desc11_128,
        Param12,
        Desc12_128,
        Param13,
        Desc13_128,
        Param14,
        Desc14_128,
        Param15,
        Desc15_128,
        Param16,
        Desc16_128,
        Param17,
        Desc17_128,
        Param18,
        Desc18_128,
        Param19,
        Desc19_128,
        Param20,
        Desc20_128,
        MaxMagicOptCount,
        ChildItemCount
    }

    public class ItemData
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
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int OffsetZ { get; set; }
        public int Speed1 { get; set; }
        public int Speed2 { get; set; }
        public int Scale { get; set; }
        public int BCHeight { get; set; }
        public int BCRadius { get; set; }
        public int EventID { get; set; }
        public string AssocFileObj128 { get; set; } = string.Empty;
        public string AssocFileDrop128 { get; set; } = string.Empty;
        public string AssocFileIcon128 { get; set; } = string.Empty;
        public string AssocFile1_128 { get; set; } = string.Empty;
        public string AssocFile2_128 { get; set; } = string.Empty;
        public int MaxStack { get; set; }
        public int ReqGender { get; set; }
        public int ReqStr { get; set; }
        public int ReqInt { get; set; }
        public int ItemClass { get; set; }
        public int SetID { get; set; }
        public float Dur_L { get; set; }
        public float Dur_U { get; set; }
        public float PD_L { get; set; }
        public float PD_U { get; set; }
        public float PDInc { get; set; }
        public float ER_L { get; set; }
        public float ER_U { get; set; }
        public float ERInc { get; set; }
        public float PAR_L { get; set; }
        public float PAR_U { get; set; }
        public float PARInc { get; set; }
        public float BR_L { get; set; }
        public float BR_U { get; set; }
        public float MD_L { get; set; }
        public float MD_U { get; set; }
        public float MDInc { get; set; }
        public float MAR_L { get; set; }
        public float MAR_U { get; set; }
        public float MARInc { get; set; }
        public float PDStr_L { get; set; }
        public float PDStr_U { get; set; }
        public float MDInt_L { get; set; }
        public float MDInt_U { get; set; }
        public int Quivered { get; set; }
        public int Ammo1_TID4 { get; set; }
        public int Ammo2_TID4 { get; set; }
        public int Ammo3_TID4 { get; set; }
        public int Ammo4_TID4 { get; set; }
        public int Ammo5_TID4 { get; set; }
        public int SpeedClass { get; set; }
        public int TwoHanded { get; set; }
        public int Range { get; set; }
        public float PAttackMin_L { get; set; }
        public float PAttackMin_U { get; set; }
        public float PAttackMax_L { get; set; }
        public float PAttackMax_U { get; set; }
        public float PAttackInc { get; set; }
        public float MAttackMin_L { get; set; }
        public float MAttackMin_U { get; set; }
        public float MAttackMax_L { get; set; }
        public float MAttackMax_U { get; set; }
        public float MAttackInc { get; set; }
        public float PAStrMin_L { get; set; }
        public float PAStrMin_U { get; set; }
        public float PAStrMax_L { get; set; }
        public float PAStrMax_U { get; set; }
        public float MAInt_Min_L { get; set; }
        public float MAInt_Min_U { get; set; }
        public float MAInt_Max_L { get; set; }
        public float MAInt_Max_U { get; set; }
        public float HR_L { get; set; }
        public float HR_U { get; set; }
        public float HRInc { get; set; }
        public float CHR_L { get; set; }
        public float CHR_U { get; set; }
        public int Param1 { get; set; }
        public string Desc1_128 { get; set; } = string.Empty;
        public int Param2 { get; set; }
        public string Desc2_128 { get; set; } = string.Empty;
        public int Param3 { get; set; }
        public string Desc3_128 { get; set; } = string.Empty;
        public int Param4 { get; set; }
        public string Desc4_128 { get; set; } = string.Empty;
        public int Param5 { get; set; }
        public string Desc5_128 { get; set; } = string.Empty;
        public int Param6 { get; set; }
        public string Desc6_128 { get; set; } = string.Empty;

        public int MaxMagicOptCount { get; set; }
        public int ChildItemCount { get; set; }

        // Override Equals and GetHashCode to compare based on ID and CodeName128
        public override bool Equals(object? obj)
        {
            if (obj is ItemData other)
            {
                return this.ID == other.ID && this.CodeName128 == other.CodeName128;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, CodeName128);
        }
    }
}
