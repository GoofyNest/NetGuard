/*
0x3013 | Agent | Server -> Client
Name: CharacterDataResponse

byte characterDataChunk[remainingPayload]
// Assembled layout:

// Assembled character data:
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

// Variant: CharacterDataResponse.Invoke
if (Game.ChunkedPacket == null)
{
    // Stop parsing this variant.
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/Inventory/InventoryItemState.cs
enum InventoryItemState : int
{
    Inactive = 1,
    Summoned = 2,
    Active = 3,
    Dead = 4,
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
    [Filter(0x3013)]
    public sealed class CharacterDataResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var h = client.Host()!;
                var response = new PacketHandlingResult();
                if (h.Agent.CharData == null)
                    h.Agent.CharData = new Packet(0x3013);
                var Settings = Main.Settings.Agent;
                var adminIndex = Settings.GameMasterConfig.GMs.FindIndex(m => m.Username == client.PlayerInfo.Username);
                int count1 = 0, count2 = 0;
                List<byte> packet_test = new();
                for (var i = 0; i < packet.GetBytes().Length; i++)
                {
                    var result = packet.ReadUInt8();
                    if (result == 255)
                        count1++;
                    packet_test.Add(result);
                }
        
                for (var i = 0; i < packet_test.Count; i++)
                {
                    var result = packet_test[i];
                    h.Agent.CharData.WriteUInt8(result);
                    if (Settings.GameMasterConfig.Misc.DisableGMConsole)
                    {
                        if (result == 255)
                        {
                            count2++;
                            if (count1 == count2 && adminIndex == -1)
                            {
                                for (int j = 1; j < 9; j++)
                                    h.Agent.CharData.WriteUInt8(packet_test[i + j]);
                                h.Agent.CharData.WriteUInt8(0);
                                i += 9;
                                continue;
                            }
                        }
                    }
                }
        
                response.ModifiedPackets.Add(new PacketList() { Packet = h.Agent.CharData });
                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3013 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}