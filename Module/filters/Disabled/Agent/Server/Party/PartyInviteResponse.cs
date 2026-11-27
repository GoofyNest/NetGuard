/*
0x3080 | Agent | Server -> Client
Name: PartyInviteResponse

// Variant: PartyInviteResponse.Invoke
byte type
uint playerUniqueId
switch (Game.AcceptanceRequest.Type): case InviteRequestType.Party1: case InviteRequestType.Party2:
{
    byte settings
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/InviteRequestType.cs
enum InviteRequestType : byte
{
    Party1 = 2,
    Party2 = 3,
    Resurrection1 = 4,
    GuildInvite = 5,
    UnionInvite = 6,
    Resurrection2 = 8,
    Accademy = 9,
    GuildWar = 10,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3080)]
    public sealed class PartyInviteResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3080 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
