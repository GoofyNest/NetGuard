/*
0xB0C5 | Agent | Server -> Client
Name: CosPickupAsyncResponse

// Payload fields are not identified by the available source.

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
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
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB0C5)]
    public sealed class CosPickupAsyncResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB0C5 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
