/*
0xB034 | Agent | Server -> Client
Name: InventoryOperationResponse

// Variant: InventoryOperationRequest.Invoke
byte unknown_30
if (unknown_30 != 1)
{
    // Stop parsing this variant.
}
byte operation
switch (operation): case InventoryOperation.SP_BUY_ITEM:
{
    byte tab
    byte slot
    ushort quantity
    uint npcUniqueId
}
switch (operation): case InventoryOperation.SP_SELL_ITEM:
{
    byte slot
    ushort quantity
    uint npcUniqueId
}
switch (operation): case InventoryOperation.SP_BUY_ITEM_COS:
{
    uint cosUniqueId
    if (Game.Player.Transport?.UniqueId != cosUniqueId)
    {
        // Stop parsing this variant.
    }
    byte tab
    byte slot
    ushort quantity
    uint npcUniqueId
}
switch (operation): case InventoryOperation.SP_SELL_ITEM_COS:
{
    uint cosUniqueId
    if (Game.Player.Transport?.UniqueId != cosUniqueId)
    {
        // Stop parsing this variant.
    }
    byte slot
    ushort quantity
    uint npcUniqueId
}

// Variant: InventoryOperationResponse.Invoke
byte result
if (result != 0x01)
{
    byte code
    // Stop parsing this variant.
}
byte type
switch (type): case InventoryOperation.SP_UPDATE_SLOTS_INV:
{
    byte sourceSlot
    byte destinationSlot
    ushort amount
    if (itemAtSource == null)
    {
        // Stop parsing this variant.
    }
    if (itemAtDestination == null)
    {
        if (itemAtSource.Amount != amount && amount != 0)
        {
            // Stop parsing this variant.
        }
    }
    byte unknown_52 // boolean: 0=false, nonzero=true
    if (unknown_52)
    {
        byte unknown_53
        if (unknown_53 == 0x00)
        {
            byte sourceSlot
            byte destinationSlot
            ushort amount
            if (itemAtSource == null)
            {
                // Stop parsing this variant.
            }
            if (itemAtDestination == null)
            {
                if (itemAtSource.Amount != amount && amount != 0)
                {
                    // Stop parsing this variant.
                }
            }
        }
    }
}
switch (type): case InventoryOperation.SP_UPDATE_SLOTS_CHEST:
{
    byte sourceSlot
    byte destinationSlot
    ushort amount
    if (itemAtSource == null)
    {
        // Stop parsing this variant.
    }
    if (itemAtDestination == null)
    {
        if (itemAtSource.Amount != amount && amount != 0)
        {
            // Stop parsing this variant.
        }
    }
}
switch (type): case InventoryOperation.SP_DEPOSIT_ITEM:
{
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_WITHDRAW_ITEM:
{
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_PICK_ITEM:
{
    byte destinationSlot
    if (destinationSlot == 0xFE)
    {
        uint goldAmount
        // Stop parsing this variant.
    }
    if (destinationSlot == 0xFE)
    {
        byte slot
    }
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
switch (type): case InventoryOperation.SP_DROP_ITEM:
{
    byte sourceSlot
}
switch (type): case InventoryOperation.SP_BUY_ITEM:
{
    byte tabIndex
    byte tabSlot
    if (gameVersion >= Chinese && gameVersion != Rigid)
    {
        ushort amount
        byte itemAmount
        byte destinationSlots[itemAmount]
    }
    if (!(gameVersion >= Chinese && gameVersion != Rigid))
    {
        byte itemAmount
        byte destinationSlots[itemAmount]
        ushort amount
    }
    if (npc == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_SELL_ITEM:
{
    byte sourceSlot
    ushort amount
    uint uniqueId
    byte buybackSlot
    if (itemAtSlot == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_DROP_GOLD:
{
    ulong gold
}
switch (type): case InventoryOperation.SP_DEPOSIT_GOLD:
{
    ulong gold
}
switch (type): case InventoryOperation.SP_WITHDRAW_GOLD:
{
    ulong gold
}
switch (type): case InventoryOperation.SP_ADD_ITEM_BY_SERVER:
{
    byte destinationSlot
    byte unknown_485 // Reason?
    if (destinationSlot == 0xFE)
    {
        byte slot
    }
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
switch (type): case InventoryOperation.SP_DEL_ITEM_BY_SERVER:
{
    byte sourceSlot
}
switch (type): case InventoryOperation.SP_UPDATE_SLOTS_INV_COS:
{
    uint uniqueId
    if (cos == null)
    {
        // Stop parsing this variant.
    }
    byte sourceSlot
    byte destinationSlot
    ushort amount
    if (itemAtSource == null)
    {
        // Stop parsing this variant.
    }
    if (itemAtDestination == null)
    {
        if (itemAtSource.Amount != amount && amount != 0)
        {
            // Stop parsing this variant.
        }
    }
}
switch (type): case InventoryOperation.SP_PICK_ITEM_COS:
{
    uint uniqueId
    if (cos == null)
    {
        // Stop parsing this variant.
    }
    byte destinationSlot
    if (destinationSlot == 0xFE)
    {
        uint goldAmount
        // Stop parsing this variant.
    }
    if (destinationSlot == 0xFE)
    {
        byte slot
    }
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
switch (type): case InventoryOperation.SP_DROP_ITEM_COS:
{
    uint uniqueId
    byte sourceSlot
    if (cos == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_BUY_ITEM_COS:
{
    uint uniqueId
    if (cos == null)
    {
        // Stop parsing this variant.
    }
    byte tabIndex
    byte tabSlot
    if (gameVersion >= Chinese && gameVersion != Rigid)
    {
        ushort amount
        byte itemAmount
        byte destinationSlots[itemAmount]
    }
    if (!(gameVersion >= Chinese && gameVersion != Rigid))
    {
        byte itemAmount
        byte destinationSlots[itemAmount]
        ushort amount
    }
    if (npc == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_SELL_ITEM_COS:
{
    uint uniqueId
    if (cos == null)
    {
        // Stop parsing this variant.
    }
    byte sourceSlot
    ushort amount
    uint uniqueId
    byte buybackSlot
    if (itemAtSlot == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_DEL_COSITEM_BY_SERVER:
{
    uint uniqueId
    if (cos == null)
    {
        // Stop parsing this variant.
    }
    byte sourceSlot
    byte reason
}
switch (type): case InventoryOperation.SP_BUY_CASH_ITEM:
{
    ushort refShopGroupId
    byte groupIndex
    byte tabIndex
    byte slotIndex
    byte itemCount
    if (refShopGoodObj != null && itemInfo != null)
    {
        byte itemSlots[itemCount]
        ushort quantity
        repeat for each slot in itemSlots
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
    }
}
switch (type): case InventoryOperation.SP_MOVE_ITEM_PET_PC:
{
    uint petUniqueId
    if (!Game.Player.HasActiveAbilityPet || cos.UniqueId != petUniqueId)
    {
        // Stop parsing this variant.
    }
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_MOVE_ITEM_PC_PET:
{
    uint petUniqueId
    if (!Game.Player.HasActiveAbilityPet || cos.UniqueId != petUniqueId)
    {
        // Stop parsing this variant.
    }
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_PICK_ITEM_BY_OTHER:
{
    uint uniqueId // picker uniqueId
    byte destinationSlot
    if (destinationSlot == 0xFE)
    {
        uint gold
    }
    if (!(destinationSlot == 0xFE))
    {
        if (destinationSlot == 0xFE)
        {
            byte slot
        }
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
switch (type): case InventoryOperation.SP_GUILD_CHEST_UPDATE_SLOT:
{
    byte sourceSlot
    byte destinationSlot
    ushort amount
    if (itemAtSource == null)
    {
        // Stop parsing this variant.
    }
    if (itemAtDestination == null)
    {
        if (itemAtSource.Amount != amount && amount != 0)
        {
            // Stop parsing this variant.
        }
    }
}
switch (type): case InventoryOperation.SP_GUILD_CHEST_DEPOSIT_ITEM:
{
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_GUILD_CHEST_WITHDRAW_ITEM:
{
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_GUILD_CHEST_DEPOSIT_GOLD:
{
    ulong gold
}
switch (type): case InventoryOperation.SP_GUILD_CHEST_WITHDRAW_GOLD:
{
    ulong gold
}
switch (type): case InventoryOperation.SP_RESTORE_SOLDITEM_INSHOP:
{
    byte destinationSlot
    byte sourceSlot
    ushort amount
}
switch (type): case InventoryOperation.SP_MOVE_ITEM_AVATAR_PC:
{
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
    ushort unknown_173 // amount
    byte unknown_175 // boolean: 0=false, nonzero=true
    if (unknown_175)
    {
        byte unknown_176
        if (unknown_176 == 0x23)
        {
            byte sourceSlot
            byte destinationSlot
            if (sourceItem == null)
            {
                // Stop parsing this variant.
            }
        }
    }
}
switch (type): case InventoryOperation.SP_MOVE_ITEM_PC_AVATAR:
{
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
    ushort unknown_184 // amount
    byte unknown_186 // boolean: 0=false, nonzero=true
    if (unknown_186)
    {
        byte unknown_187
        if (unknown_187 == 0x00)
        {
            byte sourceSlot
            byte destinationSlot
            if (sourceItem == null)
            {
                // Stop parsing this variant.
            }
        }
    }
}
switch (type): case InventoryOperation.SP_MOVE_ITEM_PC_JOB:
{
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
    ushort unknown_195 // amount
}
switch (type): case InventoryOperation.SP_MOVE_ITEM_JOB_PC:
{
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
    ushort unknown_202 // amount
}
switch (type): case InventoryOperation.SP_BUY_ITEM_WITH_TOKEN:
{
    byte tabIndex
    byte tabSlot
    if (gameVersion >= Chinese && gameVersion != Rigid)
    {
        ushort amount
        byte itemAmount
        byte destinationSlots[itemAmount]
    }
    if (!(gameVersion >= Chinese && gameVersion != Rigid))
    {
        byte itemAmount
        byte destinationSlots[itemAmount]
        ushort amount
    }
    if (npc == null)
    {
        // Stop parsing this variant.
    }
    byte unknown1
    byte tokenSlot
    ushort updatedTokenCount
    int unknown2
}
switch (type): case InventoryOperation.SP_PICK_SPECIAL_GOODS:
{
    uint unknown1
    byte destinationSlot
    if (destinationSlot == 0xFE)
    {
        uint goldAmount
        // Stop parsing this variant.
    }
    if (destinationSlot == 0xFE)
    {
        byte slot
    }
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
switch (type): case InventoryOperation.SP_MOVE_ITEM_BAG_TO_JOBTRANSPORT:
{
    uint cosUniqueId
    if (cos == null || cosUniqueId != cos.UniqueId)
    {
        // Stop parsing this variant.
    }
    byte sourceSlot
    byte destinationSlot
    if (sourceItem == null)
    {
        // Stop parsing this variant.
    }
}
switch (type): case InventoryOperation.SP_DELETE_BAG_ITEM_BY_SERVER:
{
    byte sourceSlot
}

// Variant: BuyItemHook.ReplacePacket
byte unknown_31
byte type
if (type == InventoryOperation.SP_SELL_ITEM_COS)
{
    uint tempCosId // COS unique ID
    byte srcSlot
}
if (type == InventoryOperation.SP_BUY_ITEM_COS)
{
    uint cosId
}
byte tab
byte slot
if (gameVersion >= Chinese && gameVersion != Rigid)
{
    ushort amount
    byte itemAmount
    byte destinationSlots[itemAmount]
}
if (!(gameVersion >= Chinese && gameVersion != Rigid))
{
    byte itemAmount
    byte destinationSlots[itemAmount]
    ushort amount
}

// Variant: BuyItemHook.ReplacePacket (written payload)
if (type == InventoryOperation.SP_SELL_ITEM_COS)
{
    byte unknown_40 // value: 1
    byte sP_DROP_ITEM_COS
    uint tempCosId
    byte srcSlot
}
repeat (var i = 0; i < itemAmount; i++)
{
    byte unknown_95 // value: 0x01
    if (cosId == 0)
    {
        byte sP_PICK_ITEM
    }
    if (!(cosId == 0))
    {
        byte sP_PICK_ITEM_COS
        uint cosId
    }
    byte destination
    if (gameVersion > Thailand)
    {
        int unknown_113 // value: 0
    }
    int iD
    switch (refItem.TypeID2): case 1: case 4: case 5:
    {
        byte optLevel
        long variance
        uint data
        byte unknown_126 // value: 0
        if (gameVersion > Thailand)
        {
            repeat (var j = 1; j <= bindingCount; j++)
            {
                short j
            }
        }
    }
    switch (refItem.TypeID2): case 2:
    {
        switch (refItem.TypeID3): case 1:
        {
            byte unknown_159 // value: 1
        }
        switch (refItem.TypeID3): case 2:
        {
            uint unknown_163 // value: 0
        }
        switch (refItem.TypeID3): case 3:
        {
            ushort unknown_167 // value: 0
        }
        switch (refItem.TypeID3): default:
        {
            if (refItem.TypeID4 == 3)
            {
                int amount
            }
        }
    }
    switch (refItem.TypeID2): default:
    {
        ushort amount
        if (refItem.TypeID3 == 11)
        {
            if (refItem.TypeID4 == 1 || refItem.TypeID4 == 2)
            {
                byte unknown_183 // value: 0
            }
            if (!(refItem.TypeID4 == 1 || refItem.TypeID4 == 2))
            {
                if (refItem.TypeID3 == 14 && refItem.TypeID4 == 2)
                {
                    byte unknown_185 // value: 0
                }
            }
        }
    }
    if (cosId != 0)
    {
        ascii playerName
    }
}

// Variant: BuyItemHook.ReplacePacket
if (type == InventoryOperation.SP_SELL_ITEM_COS)
{
    byte unknown_40 // value: 1
    byte sP_DROP_ITEM_COS
    uint tempCosId
    byte srcSlot
}

// Variant: BuyItemHook.ReplacePacket
repeat (var i = 0; i < itemAmount; i++)
{
    byte unknown_95 // value: 0x01
    if (cosId == 0)
    {
        byte sP_PICK_ITEM
    }
    if (!(cosId == 0))
    {
        byte sP_PICK_ITEM_COS
        uint cosId
    }
    byte destination
    if (gameVersion > Thailand)
    {
        int unknown_113 // value: 0
    }
    int iD
    switch (refItem.TypeID2): case 1: case 4: case 5:
    {
        byte optLevel
        long variance
        uint data
        byte unknown_126 // value: 0
        if (gameVersion > Thailand)
        {
            repeat (var j = 1; j <= bindingCount; j++)
            {
                short j
            }
        }
    }
    switch (refItem.TypeID2): case 2:
    {
        switch (refItem.TypeID3): case 1:
        {
            byte unknown_159 // value: 1
        }
    }
    switch (refItem.TypeID2): default:
    {
        ushort amount
    }
    if (cosId != 0)
    {
        ascii playerName
    }
}

// Response fields checked by CharacterInventory.MoveItem
byte result
if (result == 0x01)
{
    byte operation
    byte source
    byte destination
}

// Response fields checked by Storage.MoveItem
byte result
if (result == 0x01)
{
    byte operation
    byte source
    byte destination
    ushort amountMoved
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
}

// Library/RSBot.Core/Objects/Inventory/Item/BindingOption.cs
enum BindingOptionType : byte
{
    Socket = 1,
    AdvancedElixir = 2,
}

// Library/RSBot.Core/Objects/Cos/CosCommand.cs
enum CosCommand : byte
{
    Move = 1,
    Attack = 2,
    Pickup = 8,
    Follow = 9,
    Charm = 11,
    Cast = 13,
}

// Library/RSBot.Core/Objects/Inventory/InventoryItemState.cs
enum InventoryItemState : int
{
    Inactive = 1,
    Summoned = 2,
    Active = 3,
    Dead = 4,
}

// Library/RSBot.Core/Objects/Inventory/InventoryOperation.cs
enum InventoryOperation : byte
{
    SP_UPDATE_SLOTS_INV = 0,
    SP_UPDATE_SLOTS_CHEST = 1,
    SP_DEPOSIT_ITEM = 2,
    SP_WITHDRAW_ITEM = 3,
    SP_ADD_EXCHANGE = 4,
    SP_DEL_EXCHANGE = 5,
    SP_PICK_ITEM = 6,
    SP_DROP_ITEM = 7,
    SP_BUY_ITEM = 8,
    SP_SELL_ITEM = 9,
    SP_DROP_GOLD = 10,
    SP_DEPOSIT_GOLD = 11,
    SP_WITHDRAW_GOLD = 12,
    SP_UPDATE_EXCHANGE_GOLD = 13,
    SP_ADD_ITEM_BY_SERVER = 14,
    SP_DEL_ITEM_BY_SERVER = 15,
    SP_UPDATE_SLOTS_INV_COS = 16,
    SP_PICK_ITEM_COS = 17,
    SP_DROP_ITEM_COS = 18,
    SP_BUY_ITEM_COS = 19,
    SP_SELL_ITEM_COS = 20,
    SP_MISSING_TYPE_21 = 21,
    SP_DEL_COSITEM_BY_SERVER = 22,
    SP_MISSING_TYPE_23 = 23,
    SP_BUY_CASH_ITEM = 24,
    SP_MISSING_TYPE_25 = 25,
    SP_MOVE_ITEM_PET_PC = 26,
    SP_MOVE_ITEM_PC_PET = 27,
    SP_PICK_ITEM_BY_OTHER = 28,
    SP_GUILD_CHEST_UPDATE_SLOT = 29,
    SP_GUILD_CHEST_DEPOSIT_ITEM = 30,
    SP_GUILD_CHEST_WITHDRAW_ITEM = 31,
    SP_GUILD_CHEST_DEPOSIT_GOLD = 32,
    SP_GUILD_CHEST_WITHDRAW_GOLD = 33,
    SP_RESTORE_SOLDITEM_INSHOP = 34,
    SP_MOVE_ITEM_AVATAR_PC = 35,
    SP_MOVE_ITEM_PC_AVATAR = 36,
    SP_MISSING_TYPE_37 = 37,
    SP_MOVE_ITEM_TRADE_NOW = 38,
    SP_PUSH_ITEM_INTO_MAGIC_CUBE = 39,
    SP_POP_ITEM_FROM_MAGIC_CUBE = 40,
    SP_DEL_ITEM_IN_MAGIC_CUBE = 41,
    SP_MISSING_TYPE_42 = 42,
    SP_BUY_ITEM_WITH_TOKEN = 43,
    SP_PICK_SPECIAL_GOODS = 44,
    SP_MISSING_TYPE_45 = 45,
    SP_MOVE_ITEM_BAG_TO_JOBTRANSPORT = 46,
    SP_MOVE_ITEM_PC_JOB = 47,
    SP_MOVE_ITEM_JOB_PC = 48,
    SP_MISSING_TYPE_49 = 49,
    SP_MISSING_TYPE_50 = 50,
    SP_DELETE_BAG_ITEM_BY_SERVER = 51,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
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

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB034)]
    public sealed class InventoryOperationResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB034 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
