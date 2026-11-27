/*
0x703E | Agent | Client -> Server
Name: InventoryItemRepairRequest

// Variant: InventoryItemRepairRequest.Invoke
uint entityId
byte repairType
if (repairType == 1)
{
    byte slot
}

// Variant: ShoppingManager.RepairItems
uint uniqueId
byte unknown_402 // value: 2

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x703E)]
    public sealed class InventoryItemRepairRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
