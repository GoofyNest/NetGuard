namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x34A6)]
    public sealed class CharDataLoadingEnd : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            // Always flip these first. The player is entering the world regardless
            // of whether we could fully parse the CharData blob.
            client.Agent.InCharSelectionScreen = false;
            client.Agent.IsIngame = true;
            client.Agent.SentJoinRequest = false;

            try
            {
                Packet charData = client.Agent.CharData;
                charData.Lock();

                Custom.WriteLine($"[0x34A6] Parsing CharData, {charData.RemainingRead()} bytes remaining.");

                var Settings = Main.Settings;
                bool vSRO = Settings.ServerType.CurrentValue == "vSRO";

                var _playerInfo = client.PlayerInfo;
                var charIndex = _playerInfo.CharInfo.FindIndex(m => m.Charname == _playerInfo.CurrentCharName);

                if (charIndex < 0)
                {
                    Custom.WriteLine(
                        $"[0x34A6] Character '{_playerInfo.CurrentCharName}' not found in CharInfo list. " +
                        $"CharData parse skipped, but session state transitioned to in-game.");
                    return response;
                }

                CharacterInformation _char = _playerInfo.CharInfo[charIndex];

                // --- Header block (unconditional) ---
                //_char.ServerTime       = charData.ReadUInt32();
                _char.RefObjID = charData.ReadUInt32();
                _char.Scale = charData.ReadUInt8();
                _char.CurLevel = charData.ReadUInt8();
                Custom.WriteLine($"Char level = {_char.CurLevel}");
                _char.MaxLevel = charData.ReadUInt8();
                _char.ExpOffset = charData.ReadUInt64();
                _char.SExpOffset = charData.ReadUInt32();
                _char.RemainGold = charData.ReadUInt64();
                _char.RemainSkillPoint = charData.ReadUInt32();
                _char.RemainStatPoint = charData.ReadUInt16();
                _char.RemainHwanCount = charData.ReadUInt8();

                _char.GatheredExpPoint = charData.ReadUInt32();

                _char.Hp = charData.ReadUInt32();
                _char.Mp = charData.ReadUInt32();
                _char.AutoInverstExp = charData.ReadUInt8();
                _char.DailyPk = charData.ReadUInt8();
                _char.TotalPk = charData.ReadUInt16();
                _char.PkPenaltyPoint = charData.ReadUInt32();

                if (vSRO)
                {
                    _char.HwanLevel = charData.ReadUInt8();
                    _char.PvpCape = charData.ReadUInt8();
                }

                Custom.WriteLine($"[0x34A6] Items started, {charData.RemainingRead()} bytes remaining.");

                // --- Inventory ---
                if (!HandleItems(_char, charData, vSRO, out var itemsError))
                {
                    Custom.WriteLine($"[0x34A6] {itemsError}");
                    return response;
                }

                Custom.WriteLine($"[0x34A6] Items OK, {charData.RemainingRead()} bytes remaining.");

                if (vSRO)
                    return response;

                // --- vSRO-incompatible tail (jSRO only) ---
                var Skills = Main.Skills;

                charData.ReadUInt8(); // unkByte1

                var nextMastery = charData.ReadUInt8();
                while (nextMastery == 1)
                {
                    charData.ReadUInt32(); // mastery.ID
                    charData.ReadUInt8();  // mastery.Level
                    nextMastery = charData.ReadUInt8();
                }

                charData.ReadUInt8(); // unkByte2

                var nextSkill = charData.ReadUInt8();
                while (nextSkill == 1)
                {
                    charData.ReadUInt32(); // skill.ID
                    charData.ReadUInt8();  // skill.Enabled
                    nextSkill = charData.ReadUInt8();
                }

                Custom.WriteLine($"[0x34A6] Quests started, {charData.RemainingRead()} bytes remaining.");

                if (!HandleQuests(charData, out var questError))
                {
                    Custom.WriteLine($"[0x34A6] {questError}");
                    return response;
                }

                Custom.WriteLine($"[0x34A6] Quest OK, {charData.RemainingRead()} bytes remaining.");

                charData.ReadUInt8();  // unkByte3
                charData.ReadUInt32(); // UniqueCharID

                Custom.WriteLine($"[0x34A6] Position started, {charData.RemainingRead()} bytes remaining.");

                if (!HandlePosition(charData, out var posError))
                {
                    Custom.WriteLine($"[0x34A6] {posError}");
                    return response;
                }

                Custom.WriteLine($"[0x34A6] Position OK, {charData.RemainingRead()} bytes remaining.");

                charData.ReadUInt8();  // lifeState
                charData.ReadUInt8();  // State.unkByte0
                charData.ReadUInt8();  // motionState
                charData.ReadFloat();  // WalkSpeed
                charData.ReadFloat();  // RunSpeed
                charData.ReadFloat();  // HwanSpeed

                var stateBuffCount = charData.ReadUInt8();
                for (var i = 0; i < stateBuffCount; i++)
                {
                    var buffRefSkillId = charData.ReadUInt32();
                    charData.ReadUInt32(); // Duration

                    if (Skills.TryGetValue((int)buffRefSkillId, out var skill) &&
                        skill != null &&
                        skill.ParamsContains(1701213281))
                    {
                        charData.ReadUInt8(); // IsCreator
                    }
                }

                charData.ReadAscii(); // name
                charData.ReadAscii(); // jobName
                charData.ReadUInt8();  // jobType
                charData.ReadUInt8();  // jobLevel
                charData.ReadUInt32(); // jobExp
                charData.ReadUInt32(); // JobContribution
                charData.ReadUInt32(); // JobReward
                charData.ReadUInt8();  // pvpState
                byte onTransport = charData.ReadUInt8();  // onTransport
                charData.ReadUInt8();  // inCombat

                if (onTransport == 0x01)
                    charData.ReadUInt32();

                charData.ReadUInt8();  // pvpFlag
                charData.ReadUInt64(); // guideFlag
                charData.ReadUInt8();  // isGM


            }
            catch (SecurityException ex)
            {
                Custom.WriteLine($"[0x34A6] CharData parse failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[0x34A6] Unexpected error: {ex}");
            }

            return response;
        }

        private bool HandleItems(CharacterInformation _char, Packet charData, bool vSRO, out string? error)
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

                if (Inventory.ItemCount > 0)
                    Inventory.Items = [];

                for (var d = 0; d < Inventory.ItemCount; d++)
                {
                    Item _tempItem = new()
                    {
                        Slot = charData.ReadUInt8()
                    };

                    if (vSRO)
                    {
                        var itemRentType = charData.ReadUInt32();

                        switch (itemRentType)
                        {
                            case 1:
                                charData.ReadUInt16(); // CanDelete
                                charData.ReadUInt32(); // PeriodBeginTime
                                charData.ReadUInt32(); // PeriodEndTime
                                break;
                            case 2:
                                charData.ReadUInt16(); // CanDelete
                                charData.ReadUInt16(); // CanRecharge
                                charData.ReadUInt32(); // MeterRateTime
                                break;
                            case 3:
                                charData.ReadUInt16(); // CanDelete
                                charData.ReadUInt16(); // CanRecharge
                                charData.ReadUInt32(); // PeriodBeginTime
                                charData.ReadUInt32(); // PeriodEndTime
                                charData.ReadUInt32(); // PackingTime
                                break;
                                // 0 = no rent info
                        }
                    }

                    _tempItem.Id = charData.ReadUInt32();

                    if (!Items.TryGetValue((int)_tempItem.Id, out var _foundItem) || _foundItem is null)
                    {
                        error = $"Item {_tempItem.Id} (slot {_tempItem.Slot}, index {d}/{Inventory.ItemCount}) " +
                                $"not found in item table. CharData parse cannot continue.";
                        return false;
                    }

                    _tempItem.CodeName128 = _foundItem.CodeName128;

                    if (_foundItem.TypeID1 == 3)
                    {
                        if (_foundItem.TypeID2 == 1)
                        {
                            // ITEM_CH / ITEM_EU / AVATAR_
                            _tempItem.OptLevel = charData.ReadUInt8();
                            _tempItem.Variance = charData.ReadUInt64();
                            _tempItem.Durability = charData.ReadUInt32();
                            _tempItem.MagParamNum = charData.ReadUInt8();

                            for (var paramIndex = 0; paramIndex < _tempItem.MagParamNum; paramIndex++)
                            {
                                charData.ReadUInt32(); // magParam.Type
                                charData.ReadUInt32(); // magParam.Value
                            }

                            if (vSRO)
                            {
                                var bindingOptionType = charData.ReadUInt8();
                                var bindingOptionCount = charData.ReadUInt8();
                                for (var i = 0; i < bindingOptionCount; i++)
                                {
                                    charData.ReadUInt8();  // Slot
                                    charData.ReadUInt32(); // ID
                                    charData.ReadUInt32(); // nParam1
                                }

                                var bindingOptionType2 = charData.ReadUInt8();
                                var bindingOptionCount2 = charData.ReadUInt8();
                                for (var i = 0; i < bindingOptionCount2; i++)
                                {
                                    charData.ReadUInt8();  // Slot
                                    charData.ReadUInt32(); // ID
                                    charData.ReadUInt32(); // OptValue
                                }
                            }
                        }
                        else if (_foundItem.TypeID2 == 2)
                        {
                            if (_foundItem.TypeID3 == 1) // ITEM_COS_P
                            {
                                var cosState = charData.ReadUInt8();
                                if (cosState == 2 || cosState == 3 || cosState == 4)
                                {
                                    charData.ReadUInt32(); // RefCharID
                                    charData.ReadAscii();  // Name

                                    if (_foundItem.TypeID4 == 2)
                                        charData.ReadUInt32(); // SecondsToRentEndTime

                                    if (vSRO)
                                    {
                                        var hasInventoryTime = charData.ReadUInt8();
                                        if (hasInventoryTime == 0x1)
                                        {
                                            charData.ReadUInt8();  // unk
                                            charData.ReadUInt32(); // unk
                                            var unk1224 = charData.ReadUInt32();
                                            if (unk1224 == 5)
                                            {
                                                charData.ReadUInt32();
                                                charData.ReadUInt8();
                                            }
                                        }
                                    }
                                }
                            }
                            else if (_foundItem.TypeID3 == 2) // ITEM_ETC_TRANS_MONSTER
                            {
                                charData.ReadUInt32(); // RefObjID
                            }
                            else if (_foundItem.TypeID3 == 3) // MAGIC_CUBE
                            {
                                charData.ReadUInt32(); // Quantity
                            }
                        }
                        else if (_foundItem.TypeID2 == 3) // ITEM_ETC
                        {
                            charData.ReadUInt16(); // StackCount

                            if (_foundItem.TypeID3 == 11 &&
                                (_foundItem.TypeID4 == 1 || _foundItem.TypeID4 == 2))
                            {
                                charData.ReadUInt8(); // AttributeAssimilationProbability
                            }
                            else if (_foundItem.TypeID3 == 14 && _foundItem.TypeID4 == 2)
                            {
                                var magParamNum = charData.ReadUInt8();
                                for (var i = 0; i < magParamNum; i++)
                                {
                                    charData.ReadUInt32();
                                    charData.ReadUInt32();
                                }
                            }
                        }
                    }

                    Inventory.Items.Add(_tempItem);
                }

                return true;
            }
            catch (SecurityException ex)
            {
                error = $"HandleItems failed at slot index {_char.Inventory.Items?.Count ?? 0}: {ex.Message}";
                return false;
            }
        }

        private bool HandleQuests(Packet charData, out string? error)
        {
            error = null;

            try
            {
                var completedQuestCount = charData.ReadUInt16();
                charData.ReadUInt32Array(completedQuestCount);

                var activeQuestCount = charData.ReadUInt8();

                for (var activeQuestIndex = 0; activeQuestIndex < activeQuestCount; activeQuestIndex++)
                {
                    charData.ReadUInt32(); // RefQuestID
                    charData.ReadUInt8();  // AchievementCount
                    charData.ReadUInt8();  // RequiresAutoShareParty
                    var questType = charData.ReadUInt8();

                    if (questType == 28)
                        charData.ReadUInt32(); // remainingTime

                    charData.ReadUInt8(); // Status

                    if (questType != 8)
                    {
                        var questObjectiveCount = charData.ReadUInt8();
                        for (var objectiveIndex = 0; objectiveIndex < questObjectiveCount; objectiveIndex++)
                        {
                            charData.ReadUInt8();  // objective.ID
                            charData.ReadUInt8();  // objective.Status
                            charData.ReadAscii();  // objective.Name
                            var objectiveTaskCount = charData.ReadUInt8();
                            for (var taskIndex = 0; taskIndex < objectiveTaskCount; taskIndex++)
                                charData.ReadUInt32(); // task.Value
                        }
                    }

                    if (questType == 88)
                    {
                        var refObjCount = charData.ReadUInt8();
                        for (var i = 0; i < refObjCount; i++)
                            charData.ReadUInt32(); // RefObjID
                    }
                }

                return true;
            }
            catch (SecurityException ex)
            {
                error = $"HandleQuests failed: {ex.Message}";
                return false;
            }
        }

        private bool HandlePosition(Packet charData, out string? error)
        {
            error = null;

            try
            {
                var LatestRegionId = charData.ReadUInt16();
                charData.ReadFloat(); // X
                charData.ReadFloat(); // Y
                charData.ReadFloat(); // Z
                charData.ReadUInt16(); // Angle

                var movementHasDestination = charData.ReadUInt8();
                charData.ReadUInt8(); // Movement.Type

                if (movementHasDestination == 1)
                {
                    charData.ReadUInt16(); // DestinationRegion

                    if (LatestRegionId < short.MaxValue)
                    {
                        charData.ReadUInt16(); // OffsetX
                        charData.ReadUInt16(); // OffsetY
                        charData.ReadUInt16(); // OffsetZ
                    }
                    else
                    {
                        charData.ReadUInt32(); // OffsetX
                        charData.ReadUInt32(); // OffsetY
                        charData.ReadUInt32(); // OffsetZ
                    }
                }
                else
                {
                    charData.ReadUInt8();  // Source
                    charData.ReadUInt16(); // Angle
                }

                Custom.WriteLine($"[0x34A6] CharData OK, {charData.RemainingRead()} bytes remaining.");
                return true;
            }
            catch (SecurityException ex)
            {
                error = $"HandlePosition failed: {ex.Message}";
                return false;
            }
        }
    }
}