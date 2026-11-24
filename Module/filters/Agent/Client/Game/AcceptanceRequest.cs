/*
0x3080 | Agent | Client -> Server
Name: AcceptanceRequest

// Variant: AcceptanceRequest.Accept
byte unknown_39 // value: 1
byte unknown_40 // value: 1

// Variant: AcceptanceRequest.Refuse
switch (Type): case InviteRequestType.Party1: case InviteRequestType.Party2:
{
    byte unknown_56 // value: 2
    ushort unknown_57 // value: 11276
}
switch (Type): case InviteRequestType.Resurrection1: case InviteRequestType.Resurrection2:
{
    byte unknown_62 // value: 1
    byte unknown_63 // value: 2
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

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x3080)]
    public sealed class AcceptanceRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
