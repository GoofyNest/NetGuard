using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x34A6)]
    public sealed class CharDataLoadingEnd : IFilter
    {
        public enum AutoInverstType : byte { None = 0, Beginner = 1, Helpful = 2 }
        public enum PvpState { None = 0, Purple = 1, Red = 2, Unknown = 3 }
        public enum LifeState { Alive = 0x01, Dead = 0x02 }
        public enum MotionState { None = 0, Walking = 2, Running = 3, Sitting = 4 }
        public enum BodyState
        {
            None = 0, Hwan = 1, Untouchable = 2, GameMasterInvincible = 3,
            GameMasterInvisible = 4, Berzerk = 5, Stealth = 6, Invisible = 7,
        }

        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            client.Agent.InCharSelectionScreen = false;
            client.Agent.IsIngame = true;
            client.Agent.SentJoinRequest = false;

            try
            {
                var charData = h.Agent.CharData;
                if (charData is null)
                {
                    Custom.WriteLine("[0x34A6] No CharData buffer.");
                    return response;
                }
                charData.Lock();

                Custom.WriteLine($"[0x34A6] Parsing CharData, {charData.RemainingRead()} bytes remaining.");

                var Settings = Main.Settings;
                bool vSRO = Settings.ServerType.CurrentValue == "vSRO";

                var _char = h.Agent.EnsureCurrent(client.PlayerInfo.CurrentCharName);

                _char.RefObjID         = charData.ReadUInt32();
                _char.Scale            = charData.ReadUInt8();
                _char.CurLevel         = charData.ReadUInt8();
                _char.MaxLevel         = charData.ReadUInt8();
                _char.ExpOffset        = charData.ReadUInt64();
                _char.SExpOffset       = charData.ReadUInt32();
                _char.RemainGold       = charData.ReadUInt64();
                _char.RemainSkillPoint = charData.ReadUInt32();
                _char.RemainStatPoint  = charData.ReadUInt16();
                _char.RemainHwanCount  = charData.ReadUInt8();
                _char.GatheredExpPoint = charData.ReadUInt32();
                _char.Hp               = charData.ReadUInt32();
                _char.Mp               = charData.ReadUInt32();
                _char.AutoInverstExp   = charData.ReadUInt8();
                _char.DailyPk          = charData.ReadUInt8();
                _char.TotalPk          = charData.ReadUInt16();
                _char.PkPenaltyPoint   = charData.ReadUInt32();

                if (vSRO)
                {
                    _char.HwanLevel = charData.ReadUInt8();
                    _char.PvpCape   = charData.ReadUInt8();
                }

                Custom.WriteLine($"[0x34A6] Items started, {charData.RemainingRead()} bytes remaining.");
                if (!HandleItems(_char, charData, vSRO, out var itemsError))
                {
                    Custom.WriteLine($"[0x34A6] {itemsError}");
                    return response;
                }
                Custom.WriteLine($"[0x34A6] Items OK, {charData.RemainingRead()} bytes remaining.");

                if (vSRO) return response;

                var Skills = Main.Skills;

                Custom.WriteLine($"[0x34A6] Skills started, {charData.RemainingRead()} bytes remaining.");
                if (!HandleSkills(charData, out var skillsError))
                {
                    Custom.WriteLine($"[0x34A6] {skillsError}");
                    return response;
                }
                Custom.WriteLine($"[0x34A6] Skills OK, {charData.RemainingRead()} bytes remaining.");

                Custom.WriteLine($"[0x34A6] Quests started, {charData.RemainingRead()} bytes remaining.");
                if (!HandleQuests(charData, out var questError))
                {
                    Custom.WriteLine($"[0x34A6] {questError}");
                    return response;
                }
                Custom.WriteLine($"[0x34A6] Quest OK, {charData.RemainingRead()} bytes remaining.");

                charData.ReadUInt8();
                charData.ReadUInt32();

                Custom.WriteLine($"[0x34A6] Position started, {charData.RemainingRead()} bytes remaining.");
                if (!HandlePosition(charData, out var posError))
                {
                    Custom.WriteLine($"[0x34A6] {posError}");
                    return response;
                }
                Custom.WriteLine($"[0x34A6] Position OK, {charData.RemainingRead()} bytes remaining.");

                charData.ReadUInt8();
                charData.ReadUInt8();
                charData.ReadUInt8();
                charData.ReadFloat();
                charData.ReadFloat();
                charData.ReadFloat();

                var stateBuffCount = charData.ReadUInt8();
                for (var i = 0; i < stateBuffCount; i++)
                {
                    var buffRefSkillId = charData.ReadUInt32();
                    charData.ReadUInt32();
                    if (Skills.TryGetValue((int)buffRefSkillId, out var skill) &&
                        skill != null &&
                        skill.ParamsContains(1701213281))
                    {
                        charData.ReadUInt8();
                    }
                }

                charData.ReadAscii();
                charData.ReadAscii();
                charData.ReadUInt8();
                charData.ReadUInt8();
                charData.ReadUInt32();
                charData.ReadUInt32();
                charData.ReadUInt32();
                charData.ReadUInt8();
                byte onTransport = charData.ReadUInt8();
                charData.ReadUInt8();
                if (onTransport == 0x01) charData.ReadUInt32();
                charData.ReadUInt8();
                charData.ReadUInt64();
                charData.ReadUInt8();
            }
            catch (SecurityException ex) { Custom.WriteLine($"[0x34A6] CharData parse failed: {ex.Message}"); }
            catch (Exception ex) { Custom.WriteLine($"[0x34A6] Unexpected error: {ex}"); }

            return response;
        }

        private bool HandleItems(CharState _char, Packet charData, bool vSRO, out string? error)
        {
            error = null;
            _char.Inventory = new();
            var Inventory = _char.Inventory;

            try
            {
                Inventory.Size = charData.ReadUInt8();
                Inventory.ItemCount = charData.ReadUInt8();

                var Items = Main.Items;
                if (Items.Count == 0)
                {
                    error = $"Item table is empty. Extract itemdata from client and place it in {Main.Settings.Data.Path}.";
                    return false;
                }

                for (var d = 0; d < Inventory.ItemCount; d++)
                {
                    ItemState _tempItem = new() { Slot = charData.ReadUInt8() };

                    if (vSRO)
                    {
                        var itemRentType = charData.ReadUInt32();
                        switch (itemRentType)
                        {
                            case 1:
                                charData.ReadUInt16(); charData.ReadUInt32(); charData.ReadUInt32(); break;
                            case 2:
                                charData.ReadUInt16(); charData.ReadUInt16(); charData.ReadUInt32(); break;
                            case 3:
                                charData.ReadUInt16(); charData.ReadUInt16();
                                charData.ReadUInt32(); charData.ReadUInt32(); charData.ReadUInt32(); break;
                        }
                    }

                    _tempItem.Id = charData.ReadUInt32();

                    if (!Items.TryGetValue((int)_tempItem.Id, out var record) || record is null)
                    {
                        error = $"Item {_tempItem.Id} (slot {_tempItem.Slot}, index {d}/{Inventory.ItemCount}) not found in item table.";
                        return false;
                    }

                    _tempItem.CodeName128 = record.CodeName128;

                    bool IsStackable        = record.TypeID2 == 3;
                    bool IsEquip            = record.TypeID2 == 1;
                    bool IsFellowEquip      = record.TypeID2 == 5;
                    bool IsJobEquip         = record.TypeID2 == 4;
                    bool IsPet              = record.TypeID2 == 2 && record.TypeID3 == 1;
                    bool IsTransmonster     = record.TypeID2 == 2 && record.TypeID3 == 2;
                    bool IsMagicCube        = record.TypeID2 == 2 && record.TypeID3 == 3;
                    bool IsTrading          = IsStackable && record.TypeID3 == 8;
                    bool IsSpecialtyGoodBox = IsTrading && record.TypeID4 == 3;

                    if (IsEquip || IsFellowEquip || IsJobEquip)
                    {
                        _tempItem.OptLevel    = charData.ReadUInt8();
                        _tempItem.Variance    = charData.ReadUInt64();
                        _tempItem.Durability  = charData.ReadUInt32();
                        _tempItem.MagParamNum = charData.ReadUInt8();
                        for (var iMagicOption = 0; iMagicOption < _tempItem.MagParamNum; iMagicOption++)
                        {
                            charData.ReadUInt32();
                            charData.ReadUInt32();
                        }

                        if (vSRO)
                        {
                            var count1 = charData.ReadUInt8();
                            charData.ReadUInt8();
                            for (var i = 0; i < count1; i++) { charData.ReadUInt8(); charData.ReadUInt32(); charData.ReadUInt32(); }
                            var count2 = charData.ReadUInt8();
                            charData.ReadUInt8();
                            for (var i = 0; i < count2; i++) { charData.ReadUInt8(); charData.ReadUInt32(); charData.ReadUInt32(); }
                        }
                    }
                    else if (IsPet)
                    {
                        var cosState = charData.ReadUInt8();
                        if (cosState == 2 || cosState == 3 || cosState == 4)
                        {
                            charData.ReadUInt32();
                            charData.ReadAscii();
                            if (record.TypeID4 == 2)
                            {
                                if (!vSRO) charData.ReadUInt32();
                                else
                                {
                                    if (charData.ReadUInt8() == 0x1)
                                    {
                                        charData.ReadUInt8(); charData.ReadUInt32();
                                        if (charData.ReadUInt32() == 5) { charData.ReadUInt32(); charData.ReadUInt8(); }
                                    }
                                }
                            }
                        }
                    }
                    else if (IsTransmonster)     charData.ReadUInt32();
                    else if (IsMagicCube)        charData.ReadUInt32();
                    else if (IsSpecialtyGoodBox) charData.ReadUInt32();
                    else if (IsStackable)
                    {
                        charData.ReadUInt16();
                        if (record.TypeID3 == 11)
                        {
                            if (record.TypeID4 == 1 || record.TypeID4 == 2) charData.ReadUInt8();
                        }
                        else if (record.TypeID3 == 14 && record.TypeID4 == 2)
                        {
                            var magParamNum = charData.ReadUInt8();
                            for (var i = 0; i < magParamNum; i++) { charData.ReadUInt32(); charData.ReadUInt32(); }
                        }
                    }

                    Inventory.Items.Add(_tempItem);
                }

                return true;
            }
            catch (SecurityException ex)
            {
                error = $"HandleItems failed at slot index {_char.Inventory.Items.Count}: {ex.Message}";
                return false;
            }
        }

        private bool HandleSkills(Packet charData, out string? error)
        {
            error = null;
            try
            {
                charData.ReadUInt8();
                
                var nextMastery = charData.ReadUInt8();
                while (nextMastery == 1)
                {
                    charData.ReadUInt32(); 

                    charData.ReadUInt8(); 

                    nextMastery = charData.ReadUInt8();
                }

                charData.ReadUInt8();

                var nextSkill = charData.ReadUInt8();
                while (nextSkill == 1)
                {
                    charData.ReadUInt32(); 
                    
                    charData.ReadUInt8(); 

                    nextSkill = charData.ReadUInt8();
                }

                return true;
            }
            catch (SecurityException ex) { error = $"HandleSkills failed: {ex.Message}"; return false; }
        }

        private bool HandleQuests(Packet charData, out string? error)
        {
            error = null;
            try
            {
                var completedQuestCount = charData.ReadUInt16();
                for (var i = 0; i < completedQuestCount; i++) charData.ReadUInt32();

                var activeQuestCount = charData.ReadUInt8();
                for (var a = 0; a < activeQuestCount; a++)
                {
                    charData.ReadUInt32(); charData.ReadUInt8(); charData.ReadUInt8();
                    const byte QuestTypeTime = 0x04, QuestTypeStatus = 0x08, QuestTypeObjective = 0x10, QuestTypeRefObjs = 0x40;
                    var questType = charData.ReadUInt8();
                    if ((questType & QuestTypeTime) != 0) charData.ReadUInt32();
                    if ((questType & QuestTypeStatus) != 0) charData.ReadUInt8();
                    if ((questType & QuestTypeObjective) != 0)
                    {
                        var oc = charData.ReadUInt8();
                        for (var o = 0; o < oc; o++)
                        {
                            charData.ReadUInt8(); charData.ReadUInt8(); charData.ReadAscii();
                            var tc = charData.ReadUInt8();
                            for (var t = 0; t < tc; t++) charData.ReadUInt32();
                        }
                    }
                    if ((questType & QuestTypeRefObjs) != 0)
                    {
                        var rc = charData.ReadUInt8();
                        for (var r = 0; r < rc; r++) charData.ReadUInt32();
                    }
                }
                return true;
            }
            catch (SecurityException ex) { error = $"HandleQuests failed: {ex.Message}"; return false; }
        }

        private bool HandlePosition(Packet charData, out string? error)
        {
            error = null;
            try
            {
                var LatestRegionId = charData.ReadUInt16();
                charData.ReadFloat(); charData.ReadFloat(); charData.ReadFloat(); charData.ReadUInt16();
                var movementHasDestination = charData.ReadUInt8();
                charData.ReadUInt8();
                bool IsDungeon = (LatestRegionId & 0x8000) != 0;
                if (movementHasDestination == 1)
                {
                    charData.ReadUInt16();
                    if (!IsDungeon) { charData.ReadUInt16(); charData.ReadUInt16(); charData.ReadUInt16(); }
                    else            { charData.ReadUInt32(); charData.ReadUInt32(); charData.ReadUInt32(); }
                }
                else
                {
                    charData.ReadUInt8(); charData.ReadUInt16();
                }
                Custom.WriteLine($"[0x34A6] CharData OK, {charData.RemainingRead()} bytes remaining.");
                return true;
            }
            catch (SecurityException ex) { error = $"HandlePosition failed: {ex.Message}"; return false; }
        }
    }
}