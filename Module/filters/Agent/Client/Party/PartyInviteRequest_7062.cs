/*
0x7062 | Agent | Client -> Server
Name: PartyInviteRequest_7062

// Variant: Party.Invite
if (!(!IsInParty))
{
    uint playerUniqueId
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
    [Filter(0x7062)]
    public sealed class PartyInviteRequest_7062 : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
