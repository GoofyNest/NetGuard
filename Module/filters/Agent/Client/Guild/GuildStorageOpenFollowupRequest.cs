/*
0x7252 | Agent | Client -> Server
Name: GuildStorageOpenFollowupRequest

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
    [Filter(0x7252)]
    public sealed class GuildStorageOpenFollowupRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
