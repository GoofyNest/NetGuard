/*
0x703C | Agent | Client -> Server
Name: StorageOpenRequest

// Variant: StorageOpenRequest.Invoke
uint entityId

// Variant: ShoppingManager.OpenStorage
int uniqueId
byte unknown_661 // value: 0

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x703C)]
    public sealed class StorageOpenRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
