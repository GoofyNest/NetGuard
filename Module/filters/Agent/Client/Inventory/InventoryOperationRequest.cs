/*
0x7034 | Agent | Client -> Server
Name: InventoryOperationRequest

// Variant: ShoppingManager.SellItem
byte unknown_239 // value: cos == null ? InventoryOperation.SP_SELL_ITEM : InventoryOperation.SP_SELL_ITEM_COS
if (cos != null)
{
    uint uniqueId
}
byte itemSlot
ushort amount
uint uniqueId

// Variant: ShoppingManager.PurchaseItem
byte sP_BUY_ITEM // Buy item flag
byte tab
byte slot
ushort amount
uint uniqueId

// Variant: ShoppingManager.PurchaseItem
byte sP_BUY_ITEM_COS // Buy item flag
uint unknown_298 // value: 0
byte tab
byte slot
ushort amount
uint uniqueId

// Variant: ShoppingManager.StoreItem
byte unknown_741 // value: npc.Record.CodeName.Contains("WAREHOUSE") ? 0x02 : 0x1E
byte itemSlot
byte destinationSlot
uint uniqueId

// Variant: CharacterInventory.MoveItem
byte unknown_134 // value: 0
byte sourceSlot
byte destinationSlot
ushort amount

// Variant: Main.moveToPetToolStripMenuItem_Click
byte sP_MOVE_ITEM_PC_PET
uint uniqueId
byte inventoryItemSlot
byte freeSlot

// Variant: Main.moveToPlayerToolStripMenuItem_Click
byte sP_MOVE_ITEM_PET_PC
uint uniqueId
byte inventoryItemSlot
byte freeSlot

// Variant: Ability.MoveItemToPlayer
byte unknown_46 // value: 0x1A
uint uniqueId
byte slot
byte destinationSlot

// Variant: Cos.PurchaseItemFromNpc
byte sP_BUY_ITEM_COS // Buy item flag
uint uniqueId
byte tab
byte slot
ushort amount
uint uniqueId

// Variant: Cos.SellItemToNpc
byte sP_SELL_ITEM_COS
int uniqueId
byte itemSlot
ushort amount
uint uniqueId

// Variant: InventoryItem.Drop
if (cos)
{
    byte sP_DROP_ITEM_COS
    uint cosUniqueId
}
if (!(cos))
{
    byte sP_DROP_ITEM
}
byte slot

// Variant: Storage.MoveItem
byte unknown_51 // value: npc.Record.CodeName.Contains("WAREHOUSE") ? 0x01 : 0x1D
byte sourceSlot
byte destinationSlot
ushort amount
uint uniqueId

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
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

// Library/RSBot.Core/Client/ReferenceObjects/ObjectDropType.cs
[Flags]
enum ObjectDropType : byte
{
    No = 0,
    Death = 1,
    Player = 2,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

using NetGuard.Hot;
namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7034)]
    public sealed class InventoryOperationRequest : IFilter
    {
        /*
            [S -> C][0xB045] (Click / Select vendor)
            byte status = 0x01
            uint32 Vendor UniqueID
            byte unknown = 0x0
            uint32 unknown2 = 05 00 00 00
            byte unknown3 = 0x0

            [C -> S][0x7046] (Buy from vendor)
            uint32 Vendor UniqueID

            [S -> C][0xB046] (Success)
            byte status = 0x01

            ---------------------------------------------------------------

            Selling to vendor

            [C -> S][0x7034] (Client send moving item to vendor)
            byte destination = 0x09
            byte slot = 0x0D
            uint16 = 04 00
            uint32 Vendor UniqueID

            [S -> C][0x304E] (Server giving gold? I guess)
            byte status = 0x01
            uint64 goldAmount
            byte unknown = 0x0

            [S -> C][0xB034] (Server confirm moving item)
            byte status = 0x01
            byte destination = 0x09
            byte slot = 0x0D
            uint16 = 04 00
            uint32 Vendor UniqueID
            byte unknown = 0x05


        */
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            return response;
        }
    }
}