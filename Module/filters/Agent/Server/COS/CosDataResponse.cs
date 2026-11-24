/*
0x30C8 | Agent | Server -> Client
Name: CosDataResponse

// Variant: CosDataResponse.Invoke
uint uniqueId
uint objectId
if (objChar.TypeID2 == 2 && objChar.TypeID3 == 3)
{
    int hp
    int maxHp
    switch (objChar.TypeID4): case 2:
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
        uint ownerUniqueId
    }
    switch (objChar.TypeID4): case 3:
    {
        long experience
        byte level
        ushort currentHungerPoints
        int settings
        ascii name
        byte unknown_51 // ??
        uint ownerUniqueId
        byte unknown_53 // inventorySlot
    }
    switch (objChar.TypeID4): case 4:
    {
        int settings
        ascii name
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
        uint ownerUniqueId
        byte unknown_19 // inventorySlot
    }
    switch (objChar.TypeID4): case 9:
    {
        long experience
        byte level
        int satiety
        ushort unknown1
        int storedSp
        int unknown2
        int settings
        ascii name
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
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/Inventory/Item/BindingOption.cs
enum BindingOptionType : byte
{
    Socket = 1,
    AdvancedElixir = 2,
}

// Library/RSBot.Core/Objects/Inventory/InventoryItemState.cs
enum InventoryItemState : int
{
    Inactive = 1,
    Summoned = 2,
    Active = 3,
    Dead = 4,
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
    [Filter(0x30C8)]
    public sealed class CosDataResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                NetGuard.Hot.InventoryTracking.Apply(packet, client);
                try
                {
                    if (packet.RemainingRead() >= 8 && client.CurrentChar()is { } character)
                    {
                        uint uniqueId = packet.ReadUInt32();
                        uint objectId = packet.ReadUInt32();
                        PetTracking.Spawn(character, uniqueId, objectId);
                    }
                }
                catch
                {
                }
        
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x30C8 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
