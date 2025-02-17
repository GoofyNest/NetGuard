namespace Module.Helpers.ClientData
{
    public enum SkillField
    {
        Service,
        ID,
        GroupID,
        Basic_Code,
        Basic_Name,
        Basic_Group,
        Basic_Original,
        Basic_Level,
        Basic_Activity,
        Basic_ChainCode,
        Basic_RecycleCost,
        Action_PreparingTime,
        Action_CastingTime,
        Action_ActionDuration,
        Action_ReuseDelay,
        Action_CoolTime,
        Action_FlyingSpeed,
        Action_Interruptable,
        Action_Overlap,
        Action_AutoAttackType,
        Action_InTown,
        Action_Range,
        Target_Required,
        TargetType_Animal,
        TargetType_Land,
        TargetType_Building,
        TargetGroup_Self,
        TargetGroup_Ally,
        TargetGroup_Party,
        TargetGroup_Enemy_M,
        TargetGroup_Enemy_P,
        TargetGroup_Neutral,
        TargetGroup_DontCare,
        TargetEtc_SelectDeadBody,
        ReqCommon_Mastery1,
        ReqCommon_Mastery2,
        ReqCommon_MasteryLevel1,
        ReqCommon_MasteryLevel2,
        ReqCommon_Str,
        ReqCommon_Int,
        ReqLearn_Skill1,
        ReqLearn_Skill2,
        ReqLearn_Skill3,
        ReqLearn_SkillLevel1,
        ReqLearn_SkillLevel2,
        ReqLearn_SkillLevel3,
        ReqLearn_SP,
        ReqLearn_Race,
        Req_Restriction1,
        Req_Restriction2,
        ReqCast_Weapon1,
        ReqCast_Weapon2,
        Consume_HP,
        Consume_MP,
        Consume_HPRatio,
        Consume_MPRatio,
        Consume_WHAN,
        UI_SkillTab,
        UI_SkillPage,
        UI_SkillColumn,
        UI_SkillRow,
        UI_IconFile,
        UI_SkillName,
        UI_SkillToolTip,
        UI_SkillToolTip_Desc,
        UI_SkillStudy_Desc,
        AI_AttackChance,
        AI_SkillType,
        Param1,
        Param2,
        Param3,
        Param4,
        Param5,
        Param6,
        Param7,
        Param8,
        Param9,
        Param10,
        Param11,
        Param12,
        Param13,
        Param14,
        Param15,
        Param16,
        Param17,
        Param18,
        Param19,
        Param20,
        Param21,
        Param22,
        Param23,
        Param24,
        Param25,
        Param26,
        Param27,
        Param28,
        Param29,
        Param30,
        Param31,
        Param32,
        Param33,
        Param34,
        Param35,
        Param36,
        Param37,
        Param38,
        Param39,
        Param40,
        Param41,
        Param42,
        Param43,
        Param44,
        Param45,
        Param46,
        Param47,
        Param48,
        Param49,
        Param50
    }

    public class SkillData
    {
        public int Service { get; set; }
        public int ID { get; set; }
        public int GroupID { get; set; }
        public string Basic_Code { get; set; } = string.Empty;
        public string Basic_Name { get; set; } = string.Empty;
        public string Basic_Group { get; set; } = string.Empty;
        public int Basic_Original { get; set; }
        public int Basic_Level { get; set; }
        public int Basic_Activity { get; set; }
        public int Basic_ChainCode { get; set; }
        public int Basic_RecycleCost { get; set; }
        public int Action_PreparingTime { get; set; }
        public int Action_CastingTime { get; set; }
        public int Action_ActionDuration { get; set; }
        public int Action_ReuseDelay { get; set; }
        public int Action_CoolTime { get; set; }
        public int Action_FlyingSpeed { get; set; }
        public int Action_Interruptable { get; set; }
        public int Action_Overlap { get; set; }
        public int Action_AutoAttackType { get; set; }
        public int Action_InTown { get; set; }
        public int Action_Range { get; set; }
        public int Target_Required { get; set; }
        public int TargetType_Animal { get; set; }
        public int TargetType_Land { get; set; }
        public int TargetType_Building { get; set; }
        public int TargetGroup_Self { get; set; }
        public int TargetGroup_Ally { get; set; }
        public int TargetGroup_Party { get; set; }
        public int TargetGroup_Enemy_M { get; set; }
        public int TargetGroup_Enemy_P { get; set; }
        public int TargetGroup_Neutral { get; set; }
        public int TargetGroup_DontCare { get; set; }
        public int TargetEtc_SelectDeadBody { get; set; }
        public int ReqCommon_Mastery1 { get; set; }
        public int ReqCommon_Mastery2 { get; set; }
        public int ReqCommon_MasteryLevel1 { get; set; }
        public int ReqCommon_MasteryLevel2 { get; set; }
        public int ReqCommon_Str { get; set; }
        public int ReqCommon_Int { get; set; }
        public int ReqLearn_Skill1 { get; set; }
        public int ReqLearn_Skill2 { get; set; }
        public int ReqLearn_Skill3 { get; set; }
        public int ReqLearn_SkillLevel1 { get; set; }
        public int ReqLearn_SkillLevel2 { get; set; }
        public int ReqLearn_SkillLevel3 { get; set; }
        public int ReqLearn_SP { get; set; }
        public int ReqLearn_Race { get; set; }
        public int Req_Restriction1 { get; set; }
        public int Req_Restriction2 { get; set; }
        public int ReqCast_Weapon1 { get; set; }
        public int ReqCast_Weapon2 { get; set; }
        public int Consume_HP { get; set; }
        public int Consume_MP { get; set; }
        public int Consume_HPRatio { get; set; }
        public int Consume_MPRatio { get; set; }
        public int Consume_WHAN { get; set; }
        public int UI_SkillTab { get; set; }
        public int UI_SkillPage { get; set; }
        public int UI_SkillColumn { get; set; }
        public int UI_SkillRow { get; set; }
        public string UI_IconFile { get; set; } = string.Empty;
        public string UI_SkillName { get; set; } = string.Empty;
        public string UI_SkillToolTip { get; set; } = string.Empty;
        public string UI_SkillToolTip_Desc { get; set; } = string.Empty;
        public string UI_SkillStudy_Desc { get; set; } = string.Empty;
        public int AI_AttackChance { get; set; }
        public int AI_SkillType { get; set; }
        public int Param1 { get; set; }
        public int Param2 { get; set; }
        public int Param3 { get; set; }
        public int Param4 { get; set; }
        public int Param5 { get; set; }
        public int Param6 { get; set; }
        public int Param7 { get; set; }
        public int Param8 { get; set; }
        public int Param9 { get; set; }
        public int Param10 { get; set; }
        public int Param11 { get; set; }
        public int Param12 { get; set; }
        public int Param13 { get; set; }
        public int Param14 { get; set; }
        public int Param15 { get; set; }
        public int Param16 { get; set; }
        public int Param17 { get; set; }
        public int Param18 { get; set; }
        public int Param19 { get; set; }
        public int Param20 { get; set; }
        public int Param21 { get; set; }
        public int Param22 { get; set; }
        public int Param23 { get; set; }
        public int Param24 { get; set; }
        public int Param25 { get; set; }
        public int Param26 { get; set; }
        public int Param27 { get; set; }
        public int Param28 { get; set; }
        public int Param29 { get; set; }
        public int Param30 { get; set; }
        public int Param31 { get; set; }
        public int Param32 { get; set; }
        public int Param33 { get; set; }
        public int Param34 { get; set; }
        public int Param35 { get; set; }
        public int Param36 { get; set; }
        public int Param37 { get; set; }
        public int Param38 { get; set; }
        public int Param39 { get; set; }
        public int Param40 { get; set; }
        public int Param41 { get; set; }
        public int Param42 { get; set; }
        public int Param43 { get; set; }
        public int Param44 { get; set; }
        public int Param45 { get; set; }
        public int Param46 { get; set; }
        public int Param47 { get; set; }
        public int Param48 { get; set; }
        public int Param49 { get; set; }
        public int Param50 { get; set; }

        public bool ParamsContains(int value)
        {
            int[] parameters =
            {
                Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10,
                Param11, Param12, Param13, Param14, Param15, Param16, Param17, Param18, Param19, Param20,
                Param21, Param22, Param23, Param24, Param25, Param26, Param27, Param28, Param29, Param30,
                Param31, Param32, Param33, Param34, Param35, Param36, Param37, Param38, Param39, Param40,
                Param41, Param42, Param43, Param44, Param45, Param46, Param47, Param48, Param49, Param50
            };

            return parameters.Contains(value);
        }
    }
}
