/*
0x34A6 | Agent | Server -> Client
Name: CharacterDataEndResponse

// End marker. The fields below are read from the assembled 0x3013 buffer.

// Variant: CharacterDataEndResponse.Invoke
if (gameVersion >= Thailand)
{
    uint unknown_40 // serverTimestamp
}
uint modelId // RefObjID
byte scale
byte level
byte maxLevel
long experience
uint skillExperience
ulong gold
uint skillPoints
ushort statPoints
byte berzerkPoints
uint experienceChunk
int health
int mana
byte autoInverstExperience
if (gameVersion == Chinese_Old)
{
    ushort dailyPK
}
if (!(gameVersion == Chinese_Old))
{
    byte dailyPK
}
ushort totalPK
uint pKPenaltyPoint
if (gameVersion >= Thailand)
{
    byte berzerkLevel
}
if (gameVersion > Thailand)
{
    byte unknown_78 // not in jSRO
}
if (gameVersion >= Chinese)
{
    if (gameVersion != Chinese)
    {
        byte unknown_83
    }
    uint unknown_85 // You can use VIP service until this time
    byte unknown_86
    if (gameVersion == Turkey || gameVersion == VTC_Game || gameVersion == RuSro || gameVersion == Taiwan)
    {
        uint unknown_94
    }
    if (gameVersion == Rigid)
    {
        byte unknown_97[12]
    }
    if (gameVersion == VTC_Game)
    {
        byte unknown_100 // ??
    }
    if (gameVersion == Taiwan)
    {
        byte unknown_103[5]
    }
    byte serverCap
    if (gameVersion != Korean && gameVersion != Chinese && gameVersion != Japanese)
    {
        ushort unknown_113
    }
}
byte capacity
if (Capacity <= 0)
{
    // Stop parsing this variant.
}
byte amount
repeat (var i = 0; i < amount; i++)
{
    byte slot
    if (gameVersion != jSRO)
    {
        if (gameVersion > Thailand)
        {
            uint type
            switch (result.Type): case 1:
            {
                ushort canDelete
                if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                {
                    ulong periodBeginTime
                    ulong periodEndTime
                }
                if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                {
                    uint periodBeginTime
                    uint periodEndTime
                }
            }
            switch (result.Type): case 2:
            {
                ushort canDelete
                ushort canRecharge
                uint meterRateTime
            }
            switch (result.Type): case 3:
            {
                ushort canDelete
                ushort canRecharge
                if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                {
                    ulong periodBeginTime
                    ulong periodEndTime
                    ulong packingTime
                }
                if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                {
                    uint periodBeginTime
                    uint periodEndTime
                    uint packingTime
                }
            }
        }
    }
    uint itemId
    if (record.IsEquip || record.IsFellowEquip || record.IsJobEquip)
    {
        byte optLevel
        ulong value
        uint durability
        byte magicOptionsAmount
        repeat (var iMagicOption = 0; iMagicOption < magicOptionsAmount; iMagicOption++)
        {
            uint id
            uint value
        }
        if (gameVersion != jSRO && gameVersion > Thailand)
        {
            repeat (var bindingIndex = 0; bindingIndex < bindingCount; bindingIndex++)
            {
                byte bindingType
                byte bindingAmount
                repeat (var iSocketAmount = 0; iSocketAmount < bindingAmount; iSocketAmount++)
                {
                    byte slot
                    uint id
                    uint value
                }
            }
        }
    }
    if (!(record.IsEquip || record.IsFellowEquip || record.IsJobEquip))
    {
        if (record.IsPet)
        {
            byte state
            if (item.State != InventoryItemState.Inactive)
            {
                uint id // RefCharID
                ascii cosName
                if (record.TypeID4 == 2)
                {
                    if (gameVersion == jSRO)
                    {
                        uint unknown_372 // SecondsToRentEndTime
                    }
                    if (!(gameVersion == jSRO))
                    {
                        uint type
                        switch (result.Type): case 1:
                        {
                            ushort canDelete
                            if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                            {
                                ulong periodBeginTime
                                ulong periodEndTime
                            }
                            if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                            {
                                uint periodBeginTime
                                uint periodEndTime
                            }
                        }
                        switch (result.Type): case 2:
                        {
                            ushort canDelete
                            ushort canRecharge
                            uint meterRateTime
                        }
                        switch (result.Type): case 3:
                        {
                            ushort canDelete
                            ushort canRecharge
                            if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                            {
                                ulong periodBeginTime
                                ulong periodEndTime
                                ulong packingTime
                            }
                            if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                            {
                                uint periodBeginTime
                                uint periodEndTime
                                uint packingTime
                            }
                        }
                    }
                }
                if (!(record.TypeID4 == 2))
                {
                    if (gameVersion != jSRO)
                    {
                        if (gameVersion >= Chinese_Old)
                        {
                            byte level // cos level
                        }
                        byte buffCount
                        repeat (var i = 0; i < buffCount; i++)
                        {
                            byte buffType
                            if (buffType == 0 || buffType == 20 || buffType == 6)
                            {
                                uint itemId // buffType: 0 => skillId ? 20 => itemId
                                uint leftTime
                            }
                            if (buffType == 5)
                            {
                                uint itemId
                                uint leftTime
                                uint leftTime2
                                byte unk2
                            }
                        }
                    }
                }
            }
        }
        if (!(record.IsPet))
        {
            if (record.IsTransmonster)
            {
                uint id // Monster ObjectId
            }
            if (!(record.IsTransmonster))
            {
                if (record.IsMagicCube)
                {
                    uint amount // Quantity
                }
                if (!(record.IsMagicCube))
                {
                    if (record.IsSpecialtyGoodBox)
                    {
                        uint amount // Quantity
                    }
                    if (!(record.IsSpecialtyGoodBox))
                    {
                        if (record.IsStackable)
                        {
                            ushort amount
                            if (record.TypeID3 == 11)
                            {
                                if (record.TypeID4 == 1 || record.TypeID4 == 2)
                                {
                                    byte unknown_423
                                }
                            }
                            if (!(record.TypeID3 == 11))
                            {
                                if (record.TypeID3 == 14 && record.TypeID4 == 2)
                                {
                                    byte magParamCount
                                    repeat (var i = 0; i < magParamCount; i++)
                                    {
                                        uint unknown_432
                                        uint unknown_433
                                    }
                                }
                            }
                            if (record.IsTrading && gameVersion != jSRO)
                            {
                                ascii unknown_439
                            }
                        }
                    }
                }
            }
        }
    }
}
if (gameVersion >= Thailand)
{
    byte capacity
    if (Capacity <= 0)
    {
        // Stop parsing this variant.
    }
    byte amount
    repeat (var i = 0; i < amount; i++)
    {
        byte slot
        if (gameVersion != jSRO)
        {
            if (gameVersion > Thailand)
            {
                uint type
                switch (result.Type): case 1:
                {
                    ushort canDelete
                    if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                    {
                        ulong periodBeginTime
                        ulong periodEndTime
                    }
                    if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                    {
                        uint periodBeginTime
                        uint periodEndTime
                    }
                }
                switch (result.Type): case 2:
                {
                    ushort canDelete
                    ushort canRecharge
                    uint meterRateTime
                }
                switch (result.Type): case 3:
                {
                    ushort canDelete
                    ushort canRecharge
                    if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                    {
                        ulong periodBeginTime
                        ulong periodEndTime
                        ulong packingTime
                    }
                    if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                    {
                        uint periodBeginTime
                        uint periodEndTime
                        uint packingTime
                    }
                }
            }
        }
        uint itemId
        if (record.IsEquip || record.IsFellowEquip || record.IsJobEquip)
        {
            byte optLevel
            ulong value
            uint durability
            byte magicOptionsAmount
            repeat (var iMagicOption = 0; iMagicOption < magicOptionsAmount; iMagicOption++)
            {
                uint id
                uint value
            }
            if (gameVersion != jSRO && gameVersion > Thailand)
            {
                repeat (var bindingIndex = 0; bindingIndex < bindingCount; bindingIndex++)
                {
                    byte bindingType
                    byte bindingAmount
                    repeat (var iSocketAmount = 0; iSocketAmount < bindingAmount; iSocketAmount++)
                    {
                        byte slot
                        uint id
                        uint value
                    }
                }
            }
        }
        if (!(record.IsEquip || record.IsFellowEquip || record.IsJobEquip))
        {
            if (record.IsPet)
            {
                byte state
                if (item.State != InventoryItemState.Inactive)
                {
                    uint id // RefCharID
                    ascii cosName
                    if (record.TypeID4 == 2)
                    {
                        if (gameVersion == jSRO)
                        {
                            uint unknown_372 // SecondsToRentEndTime
                        }
                        if (!(gameVersion == jSRO))
                        {
                            uint type
                            switch (result.Type): case 1:
                            {
                                ushort canDelete
                                if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                                {
                                    ulong periodBeginTime
                                    ulong periodEndTime
                                }
                                if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                                {
                                    uint periodBeginTime
                                    uint periodEndTime
                                }
                            }
                            switch (result.Type): case 2:
                            {
                                ushort canDelete
                                ushort canRecharge
                                uint meterRateTime
                            }
                            switch (result.Type): case 3:
                            {
                                ushort canDelete
                                ushort canRecharge
                                if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                                {
                                    ulong periodBeginTime
                                    ulong periodEndTime
                                    ulong packingTime
                                }
                                if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                                {
                                    uint periodBeginTime
                                    uint periodEndTime
                                    uint packingTime
                                }
                            }
                        }
                    }
                    if (!(record.TypeID4 == 2))
                    {
                        if (gameVersion != jSRO)
                        {
                            if (gameVersion >= Chinese_Old)
                            {
                                byte level // cos level
                            }
                            byte buffCount
                            repeat (var i = 0; i < buffCount; i++)
                            {
                                byte buffType
                                if (buffType == 0 || buffType == 20 || buffType == 6)
                                {
                                    uint itemId // buffType: 0 => skillId ? 20 => itemId
                                    uint leftTime
                                }
                                if (buffType == 5)
                                {
                                    uint itemId
                                    uint leftTime
                                    uint leftTime2
                                    byte unk2
                                }
                            }
                        }
                    }
                }
            }
            if (!(record.IsPet))
            {
                if (record.IsTransmonster)
                {
                    uint id // Monster ObjectId
                }
                if (!(record.IsTransmonster))
                {
                    if (record.IsMagicCube)
                    {
                        uint amount // Quantity
                    }
                    if (!(record.IsMagicCube))
                    {
                        if (record.IsSpecialtyGoodBox)
                        {
                            uint amount // Quantity
                        }
                        if (!(record.IsSpecialtyGoodBox))
                        {
                            if (record.IsStackable)
                            {
                                ushort amount
                                if (record.TypeID3 == 11)
                                {
                                    if (record.TypeID4 == 1 || record.TypeID4 == 2)
                                    {
                                        byte unknown_423
                                    }
                                }
                                if (!(record.TypeID3 == 11))
                                {
                                    if (record.TypeID3 == 14 && record.TypeID4 == 2)
                                    {
                                        byte magParamCount
                                        repeat (var i = 0; i < magParamCount; i++)
                                        {
                                            uint unknown_432
                                            uint unknown_433
                                        }
                                    }
                                }
                                if (record.IsTrading && gameVersion != jSRO)
                                {
                                    ascii unknown_439
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
if (!(gameVersion == jSRO) && gameVersion > Vietnam)
{
    byte capacity
    if (Capacity <= 0)
    {
        // Stop parsing this variant.
    }
    byte amount
    repeat (var i = 0; i < amount; i++)
    {
        byte slot
        if (gameVersion != jSRO)
        {
            if (gameVersion > Thailand)
            {
                uint type
                switch (result.Type): case 1:
                {
                    ushort canDelete
                    if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                    {
                        ulong periodBeginTime
                        ulong periodEndTime
                    }
                    if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                    {
                        uint periodBeginTime
                        uint periodEndTime
                    }
                }
                switch (result.Type): case 2:
                {
                    ushort canDelete
                    ushort canRecharge
                    uint meterRateTime
                }
                switch (result.Type): case 3:
                {
                    ushort canDelete
                    ushort canRecharge
                    if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                    {
                        ulong periodBeginTime
                        ulong periodEndTime
                        ulong packingTime
                    }
                    if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                    {
                        uint periodBeginTime
                        uint periodEndTime
                        uint packingTime
                    }
                }
            }
        }
        uint itemId
        if (record.IsEquip || record.IsFellowEquip || record.IsJobEquip)
        {
            byte optLevel
            ulong value
            uint durability
            byte magicOptionsAmount
            repeat (var iMagicOption = 0; iMagicOption < magicOptionsAmount; iMagicOption++)
            {
                uint id
                uint value
            }
            if (gameVersion != jSRO && gameVersion > Thailand)
            {
                repeat (var bindingIndex = 0; bindingIndex < bindingCount; bindingIndex++)
                {
                    byte bindingType
                    byte bindingAmount
                    repeat (var iSocketAmount = 0; iSocketAmount < bindingAmount; iSocketAmount++)
                    {
                        byte slot
                        uint id
                        uint value
                    }
                }
            }
        }
        if (!(record.IsEquip || record.IsFellowEquip || record.IsJobEquip))
        {
            if (record.IsPet)
            {
                byte state
                if (item.State != InventoryItemState.Inactive)
                {
                    uint id // RefCharID
                    ascii cosName
                    if (record.TypeID4 == 2)
                    {
                        if (gameVersion == jSRO)
                        {
                            uint unknown_372 // SecondsToRentEndTime
                        }
                        if (!(gameVersion == jSRO))
                        {
                            uint type
                            switch (result.Type): case 1:
                            {
                                ushort canDelete
                                if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                                {
                                    ulong periodBeginTime
                                    ulong periodEndTime
                                }
                                if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                                {
                                    uint periodBeginTime
                                    uint periodEndTime
                                }
                            }
                            switch (result.Type): case 2:
                            {
                                ushort canDelete
                                ushort canRecharge
                                uint meterRateTime
                            }
                            switch (result.Type): case 3:
                            {
                                ushort canDelete
                                ushort canRecharge
                                if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                                {
                                    ulong periodBeginTime
                                    ulong periodEndTime
                                    ulong packingTime
                                }
                                if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                                {
                                    uint periodBeginTime
                                    uint periodEndTime
                                    uint packingTime
                                }
                            }
                        }
                    }
                    if (!(record.TypeID4 == 2))
                    {
                        if (gameVersion != jSRO)
                        {
                            if (gameVersion >= Chinese_Old)
                            {
                                byte level // cos level
                            }
                            byte buffCount
                            repeat (var i = 0; i < buffCount; i++)
                            {
                                byte buffType
                                if (buffType == 0 || buffType == 20 || buffType == 6)
                                {
                                    uint itemId // buffType: 0 => skillId ? 20 => itemId
                                    uint leftTime
                                }
                                if (buffType == 5)
                                {
                                    uint itemId
                                    uint leftTime
                                    uint leftTime2
                                    byte unk2
                                }
                            }
                        }
                    }
                }
            }
            if (!(record.IsPet))
            {
                if (record.IsTransmonster)
                {
                    uint id // Monster ObjectId
                }
                if (!(record.IsTransmonster))
                {
                    if (record.IsMagicCube)
                    {
                        uint amount // Quantity
                    }
                    if (!(record.IsMagicCube))
                    {
                        if (record.IsSpecialtyGoodBox)
                        {
                            uint amount // Quantity
                        }
                        if (!(record.IsSpecialtyGoodBox))
                        {
                            if (record.IsStackable)
                            {
                                ushort amount
                                if (record.TypeID3 == 11)
                                {
                                    if (record.TypeID4 == 1 || record.TypeID4 == 2)
                                    {
                                        byte unknown_423
                                    }
                                }
                                if (!(record.TypeID3 == 11))
                                {
                                    if (record.TypeID3 == 14 && record.TypeID4 == 2)
                                    {
                                        byte magParamCount
                                        repeat (var i = 0; i < magParamCount; i++)
                                        {
                                            uint unknown_432
                                            uint unknown_433
                                        }
                                    }
                                }
                                if (record.IsTrading && gameVersion != jSRO)
                                {
                                    ascii unknown_439
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    byte capacity
    if (Capacity <= 0)
    {
        // Stop parsing this variant.
    }
    byte amount
    repeat (var i = 0; i < amount; i++)
    {
        byte slot
        if (gameVersion != jSRO)
        {
            if (gameVersion > Thailand)
            {
                uint type
                switch (result.Type): case 1:
                {
                    ushort canDelete
                    if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                    {
                        ulong periodBeginTime
                        ulong periodEndTime
                    }
                    if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                    {
                        uint periodBeginTime
                        uint periodEndTime
                    }
                }
                switch (result.Type): case 2:
                {
                    ushort canDelete
                    ushort canRecharge
                    uint meterRateTime
                }
                switch (result.Type): case 3:
                {
                    ushort canDelete
                    ushort canRecharge
                    if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                    {
                        ulong periodBeginTime
                        ulong periodEndTime
                        ulong packingTime
                    }
                    if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                    {
                        uint periodBeginTime
                        uint periodEndTime
                        uint packingTime
                    }
                }
            }
        }
        uint itemId
        if (record.IsEquip || record.IsFellowEquip || record.IsJobEquip)
        {
            byte optLevel
            ulong value
            uint durability
            byte magicOptionsAmount
            repeat (var iMagicOption = 0; iMagicOption < magicOptionsAmount; iMagicOption++)
            {
                uint id
                uint value
            }
            if (gameVersion != jSRO && gameVersion > Thailand)
            {
                repeat (var bindingIndex = 0; bindingIndex < bindingCount; bindingIndex++)
                {
                    byte bindingType
                    byte bindingAmount
                    repeat (var iSocketAmount = 0; iSocketAmount < bindingAmount; iSocketAmount++)
                    {
                        byte slot
                        uint id
                        uint value
                    }
                }
            }
        }
        if (!(record.IsEquip || record.IsFellowEquip || record.IsJobEquip))
        {
            if (record.IsPet)
            {
                byte state
                if (item.State != InventoryItemState.Inactive)
                {
                    uint id // RefCharID
                    ascii cosName
                    if (record.TypeID4 == 2)
                    {
                        if (gameVersion == jSRO)
                        {
                            uint unknown_372 // SecondsToRentEndTime
                        }
                        if (!(gameVersion == jSRO))
                        {
                            uint type
                            switch (result.Type): case 1:
                            {
                                ushort canDelete
                                if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                                {
                                    ulong periodBeginTime
                                    ulong periodEndTime
                                }
                                if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                                {
                                    uint periodBeginTime
                                    uint periodEndTime
                                }
                            }
                            switch (result.Type): case 2:
                            {
                                ushort canDelete
                                ushort canRecharge
                                uint meterRateTime
                            }
                            switch (result.Type): case 3:
                            {
                                ushort canDelete
                                ushort canRecharge
                                if (gameVersion >= Chinese_Old && gameVersion != Rigid)
                                {
                                    ulong periodBeginTime
                                    ulong periodEndTime
                                    ulong packingTime
                                }
                                if (!(gameVersion >= Chinese_Old && gameVersion != Rigid))
                                {
                                    uint periodBeginTime
                                    uint periodEndTime
                                    uint packingTime
                                }
                            }
                        }
                    }
                    if (!(record.TypeID4 == 2))
                    {
                        if (gameVersion != jSRO)
                        {
                            if (gameVersion >= Chinese_Old)
                            {
                                byte level // cos level
                            }
                            byte buffCount
                            repeat (var i = 0; i < buffCount; i++)
                            {
                                byte buffType
                                if (buffType == 0 || buffType == 20 || buffType == 6)
                                {
                                    uint itemId // buffType: 0 => skillId ? 20 => itemId
                                    uint leftTime
                                }
                                if (buffType == 5)
                                {
                                    uint itemId
                                    uint leftTime
                                    uint leftTime2
                                    byte unk2
                                }
                            }
                        }
                    }
                }
            }
            if (!(record.IsPet))
            {
                if (record.IsTransmonster)
                {
                    uint id // Monster ObjectId
                }
                if (!(record.IsTransmonster))
                {
                    if (record.IsMagicCube)
                    {
                        uint amount // Quantity
                    }
                    if (!(record.IsMagicCube))
                    {
                        if (record.IsSpecialtyGoodBox)
                        {
                            uint amount // Quantity
                        }
                        if (!(record.IsSpecialtyGoodBox))
                        {
                            if (record.IsStackable)
                            {
                                ushort amount
                                if (record.TypeID3 == 11)
                                {
                                    if (record.TypeID4 == 1 || record.TypeID4 == 2)
                                    {
                                        byte unknown_423
                                    }
                                }
                                if (!(record.TypeID3 == 11))
                                {
                                    if (record.TypeID3 == 14 && record.TypeID4 == 2)
                                    {
                                        byte magParamCount
                                        repeat (var i = 0; i < magParamCount; i++)
                                        {
                                            uint unknown_432
                                            uint unknown_433
                                        }
                                    }
                                }
                                if (record.IsTrading && gameVersion != jSRO)
                                {
                                    ascii unknown_439
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
byte unknown_43 // unknown
repeat
{
    byte hasNext_45 // Stop repetition when !(hasNext_45 == 0x01)
    uint masteryId
    byte level
}
byte unknown_48 // unknown
repeat
{
    byte hasNext_50 // Stop repetition when !(hasNext_50 == 0x01)
    uint skillId
    byte enabled // boolean: 0=false, nonzero=true
}
ushort count
repeat (var i = 0; i < count; i++)
{
    uint unknown_26
}
byte count
repeat (var i = 0; i < count; i++)
{
    uint questId
    if (gameVersion == Vietnam274)
    {
        ushort achievementAmount
        ushort requiredShareParty
    }
    if (!(gameVersion == Vietnam274))
    {
        byte achievementAmount
        byte requiredShareParty
    }
    if (gameVersion >= Chinese)
    {
        byte unknown1
        byte unknown2
    }
    byte type
    if ((activeQuest.Type & QuestType.Time) == QuestType.Time)
    {
        int remainingTime
    }
    if ((activeQuest.Type & QuestType.Status) == QuestType.Status)
    {
        byte status
    }
    if ((activeQuest.Type & QuestType.Objective) == QuestType.Objective)
    {
        byte objectiveCount
        repeat (var i = 0; i < objectiveCount; i++)
        {
            byte id
            byte inProgress // boolean: 0=false, nonzero=true
            ascii nameStrId
            byte tasksCount
            uint tasks[tasksCount]
        }
    }
    if ((activeQuest.Type & QuestType.RefObjects) == QuestType.RefObjects)
    {
        byte npcsCount
        uint npcs[npcsCount]
    }
}
byte unknown_141 // Unknown
if (!(gameVersion == jSRO) && gameVersion > Thailand)
{
    uint collectionBookStartedThemeCount
    repeat (var i = 0; i < collectionBookStartedThemeCount; i++)
    {
        uint unknown_148 // index
        uint unknown_149 // Starttime
        uint unknown_150 // pages
    }
}
uint uniqueId
ushort region
float xOffset
float zOffset
float yOffset
short angle
byte hasDestination // boolean: 0=false, nonzero=true
byte type
if (result.HasDestination)
{
    ushort region
    if (!position.Region.IsDungeon)
    {
        short xOffset
        short zOffset
        short yOffset
    }
    if (!(!position.Region.IsDungeon))
    {
        int xOffset
        int zOffset
        int yOffset
    }
}
if (!(result.HasDestination))
{
    byte unknown_114 // 0 = Spinning, 1 = Sky-/Key-walking
    short angle
}
byte lifeState
if (gameVersion == jSRO || gameVersion > Thailand)
{
    byte unknown_128 // unkByte0
}
byte motionState
if (gameVersion != jSRO)
{
    byte bodyState
}
if (gameVersion > Vietnam193)
{
    byte unknown_138 // hasRedArrowEffect
}
float walkSpeed
float runSpeed
float berzerkSpeed
byte buffCount
repeat (var i = 0; i < buffCount; i++)
{
    uint id
    uint token
    if (buff.Record.Params.Contains(1701213281))
    {
        byte unknown_159 // IsCreator // boolean: 0=false, nonzero=true
    }
}
ascii characterName
if (gameVersion == jSRO || gameVersion <= Vietnam)
{
    ascii name
    byte type
    byte level
    uint experience
    uint contribution
    uint reward
}
ascii name
byte title
byte rank
byte type
byte level
long experience
byte pvpState
byte onTransport // On transport? // boolean: 0=false, nonzero=true
byte inCombat // boolean: 0=false, nonzero=true
if (gameVersion >= Chinese)
{
    byte unknown_168
}
if (character.OnTransport)
{
    uint transportUniqueId
}
if (gameVersion >= Chinese)
{
    uint unknown_174 // unkUint2 i think it is using for balloon event or buff for events
}
if (gameVersion > Vietnam)
{
    byte unknown_177
}
byte unknown_179 // PVP dress for the CTF event //0 = Red Side, 1 = Blue Side, 0xFF = None
if (gameVersion > Chinese && gameVersion != Global && gameVersion != Rigid && gameVersion != RuSro && gameVersion != Korean && gameVersion != VTC_Game && gameVersion != Japanese)
{
    byte unknown_191 // 0xFF
    ushort unknown_192 // 0xFF
    ushort unknown_193 // 0xFF
}
if (gameVersion == jSRO)
{
    if (character.OnTransport)
    {
        uint transportUniqueId
    }
    byte unknown_203 // PVP dress for the CTF event //0 = Red Side, 1 = Blue Side, 0xFF = None
}
if (gameVersion == jSRO || gameVersion >= Thailand)
{
    ulong unknown_208
}
if (!(gameVersion == jSRO || gameVersion >= Thailand))
{
    uint unknown_210
}
if (gameVersion == Chinese_Old || gameVersion == Chinese || gameVersion == Global || gameVersion == RuSro || gameVersion == Korean || gameVersion == VTC_Game || gameVersion == Japanese)
{
    byte unknown_223
}
if (gameVersion == Chinese)
{
    byte unknown_227
}
if (gameVersion == jSRO)
{
    byte isGameMaster // boolean: 0=false, nonzero=true
}
if (!(gameVersion == jSRO))
{
    uint jID
    byte isGameMaster // boolean: 0=false, nonzero=true
}
if (!Game.Clientless)
{
    // Stop parsing this variant.
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/AutoInverstType.cs
[Flags]
enum AutoInverstType : byte
{
    None = 0,
    Beginner = 1,
    Helpful = 2,
}

// Library/RSBot.Core/Objects/Inventory/Item/BindingOption.cs
enum BindingOptionType : byte
{
    Socket = 1,
    AdvancedElixir = 2,
}

// Library/RSBot.Core/Objects/BodyState.cs
enum BodyState : int
{
    None = 0,
    Hwan = 1,
    Untouchable = 2,
    GameMasterInvincible = 3,
    GameMasterInvisible = 4,
    Berzerk = 5,
    Stealth = 6,
    Invisible = 7,
}

// Library/RSBot.Core/Objects/Inventory/InventoryItemState.cs
enum InventoryItemState : int
{
    Inactive = 1,
    Summoned = 2,
    Active = 3,
    Dead = 4,
}

// Library/RSBot.Core/Objects/JobType.cs
enum JobType : int
{
    None = 0,
    Trade = 1,
    Thief = 2,
    Hunter = 3,
}

// Library/RSBot.Core/Objects/LifeState.cs
enum LifeState : int
{
    Alive = 1,
    Dead = 2,
}

// Library/RSBot.Core/Objects/MotionState.cs
enum MotionState : int
{
    None = 0,
    Walking = 2,
    Running = 3,
    Sitting = 4,
}

// Library/RSBot.Core/Objects/MovementType.cs
enum MovementType : int
{
    Walking = 0,
    Running = 1,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}

// Library/RSBot.Core/Objects/PvpFlag.cs
enum PvpFlag : int
{
    None = 0,
    Red = 1,
    Gray = 2,
    Blue = 3,
    White = 4,
    Gold = 5,
}

// Library/RSBot.Core/Objects/PvpState.cs
enum PvpState : int
{
    None = 0,
    Purple = 1,
    Red = 2,
    Unknown = 3,
}

// Library/RSBot.Core/Objects/Quests/QuestStatus.cs
enum QuestStatus : byte
{
    Initialized = 1,
    CompletedButNotSupplied = 2,
    Completed = 3,
    CompletedXTimes = 4,
    Unavailable = 5,
    Cancelled = 6,
    StartedByUser = 7,
    CompletedByUserButNotSupplied = 8,
}

// Library/RSBot.Core/Objects/Quests/QuestType.cs
[Flags]
enum QuestType : byte
{
    UnkBit0 = 1,
    UnkBit1 = 2,
    Time = 4,
    Status = 8,
    Objective = 16,
    UnkBit5 = 32,
    RefObjects = 64,
    UnkBit7 = 128,
}

// Library/RSBot.Core/GameClientType.cs
enum GameClientType : byte
{
    jSRO = 0,
    Japanese_Old = 1,
    Thailand = 2,
    Vietnam = 3,
    Taiwan_Old = 4,
    Vietnam193 = 5,
    Vietnam274 = 6,
    Chinese_Old = 7,
    Chinese = 8,
    Global = 9,
    Turkey = 10,
    VTC_Game = 11,
    Taiwan = 12,
    Korean = 13,
    Japanese = 14,
    RuSro = 15,
    Rigid = 16,
}
*/

using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x34A6)]
    public sealed class CharacterDataEndResponse : IFilter
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

                //Custom.WriteLine($"[0x34A6] Parsing CharData, {charData.RemainingRead()} bytes remaining.");

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

                //Custom.WriteLine($"[0x34A6] Items started, {charData.RemainingRead()} bytes remaining.");
                if (!HandleItems(_char, charData, vSRO, out var itemsError))
                {
                    Custom.WriteLine($"[0x34A6] {itemsError}");
                    return response;
                }
                //Custom.WriteLine($"[0x34A6] Items OK, {charData.RemainingRead()} bytes remaining.");

                if (vSRO) 
                    return response;

                var Skills = Main.Skills;

                //Custom.WriteLine($"[0x34A6] Skills started, {charData.RemainingRead()} bytes remaining.");
                if (!HandleSkills(charData, out var skillsError))
                {
                    Custom.WriteLine($"[0x34A6] {skillsError}");
                    return response;
                }
                //Custom.WriteLine($"[0x34A6] Skills OK, {charData.RemainingRead()} bytes remaining.");

                //Custom.WriteLine($"[0x34A6] Quests started, {charData.RemainingRead()} bytes remaining.");
                if (!HandleQuests(charData, out var questError))
                {
                    Custom.WriteLine($"[0x34A6] {questError}");
                    return response;
                }
                //Custom.WriteLine($"[0x34A6] Quest OK, {charData.RemainingRead()} bytes remaining.");

                charData.ReadUInt8();
                charData.ReadUInt32();

                //Custom.WriteLine($"[0x34A6] Position started, {charData.RemainingRead()} bytes remaining.");
                if (!HandlePosition(charData, out var posError))
                {
                    Custom.WriteLine($"[0x34A6] {posError}");
                    return response;
                }
                //Custom.WriteLine($"[0x34A6] Position OK, {charData.RemainingRead()} bytes remaining.");

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

                //Custom.WriteLine($"CharData complete");
            }
            catch (SecurityException ex) { Custom.WriteLine($"[0x34A6] CharData parse failed: {ex.Message}"); }
            catch (Exception ex) { Custom.WriteLine($"[0x34A6] Unexpected error: {ex}"); }

            return response;
        }

        private bool HandleItems(CharState _char, Packet charData, bool vSRO, out string? error)
        {
            error = null;
            PetTracking.BeginSnapshot(_char);
            var pets = new List<CosState>();
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

                    bool isAttackPet = IsPet && record.TypeID4 == 1;
                    bool isGrabPet   = IsPet && record.TypeID4 == 2;

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
                        _tempItem.State = cosState;

                        //Custom.WriteLine($"{_tempItem.CodeName128} cosState: {cosState} TypeID4: {record.TypeID4} isAttack: {isAttackPet} isGrab: {isGrabPet}");

                        if (cosState == 2 || cosState == 3 || cosState == 4)
                        {
                            UInt32 RefCharID = charData.ReadUInt32(); // RefCharID
                            string petName = charData.ReadAscii();

                            if (isAttackPet || isGrabPet)
                            {
                                pets.Add(new CosState
                                {
                                    ObjectID = RefCharID,
                                    Type = isAttackPet ? 1 : 2,
                                    Summoned = cosState == 2
                                });
                            }
                            if (record.TypeID4 == 2)
                            {
                                if (!vSRO) 
                                    charData.ReadUInt32();
                                else
                                {
                                    if (charData.ReadUInt8() == 0x1)
                                    {
                                        charData.ReadUInt8(); 

                                        var unk_1 = charData.ReadUInt32();

                                        //Custom.WriteLine($"{unk_1}");

                                        if (charData.ReadUInt32() == 5) { 
                                            var unk_2 = charData.ReadUInt32(); 

                                            //Custom.WriteLine($"{unk_2}");

                                            charData.ReadUInt8(); 
                                        }
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
                        _tempItem.Amount = charData.ReadUInt16();
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

                PetTracking.CompleteSnapshot(_char, pets);
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

