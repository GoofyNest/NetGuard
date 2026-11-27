/*
0x7250 | Agent | Client -> Server
Name: ShoppingManagerOpenGuildStorageRequest

// Variant: ShoppingManager.OpenGuildStorage
if (Game.Clientless)
{
    int uniqueId
}

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
    [Filter(0x7250)]
    public sealed class ShoppingManagerOpenGuildStorageRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
