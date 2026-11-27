/*
0x3040 | Agent | Server -> Client
Name: InventoryUpdateItemResponse

// Variant: InventoryUpdateItemResponse.Invoke
byte sourceSlot
byte itemUpdateFlag
if (item == null)
{
    // Stop parsing this variant.
}
if (itemUpdateFlag.HasFlag(ItemUpdateFlag.RefObjID))
{
    uint itemId
}
if (itemUpdateFlag.HasFlag(ItemUpdateFlag.OptLevel))
{
    byte optLevel
}
if (itemUpdateFlag.HasFlag(ItemUpdateFlag.Variance))
{
    ulong value
}
if (itemUpdateFlag.HasFlag(ItemUpdateFlag.Quanity))
{
    ushort amount
}
if (itemUpdateFlag.HasFlag(ItemUpdateFlag.Durability))
{
    uint durability
}
if (itemUpdateFlag.HasFlag(ItemUpdateFlag.State))
{
    byte state
}
if (itemUpdateFlag.HasFlag(ItemUpdateFlag.MagParams))
{
    byte magParamCount
    repeat (var i = 0; i < magParamCount; i++)
    {
        uint id
        uint value
    }
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

// Library/RSBot.Core/Objects/Inventory/Item/ItemUpdateFlag.cs
[Flags]
enum ItemUpdateFlag : byte
{
    RefObjID = 1,
    OptLevel = 2,
    Unknown = 3,
    Variance = 4,
    Quanity = 8,
    Durability = 16,
    MagParams = 32,
    State = 64,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3040)]
    public sealed class InventoryUpdateItemResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3040 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
