/*
0x7060 | Agent | Client -> Server
Name: PartyInviteRequest

// Variant: Party.Invite
if (!IsInParty)
{
    uint playerUniqueId
    byte unknown_97 // value: Settings.GetPartyType()
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
    [Filter(0x7060)]
    public sealed class PartyInviteRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
