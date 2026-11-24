/*
0x3065 | Agent | Server -> Client
Name: PartyCreateFromMatching

// Variant: PartyCreateFromMatching.Invoke
byte unknown_33 // FF
if (gameVersion > Thailand)
{
    uint unknown_36 // partyId
}
uint leaderId
byte settings
byte memberAmount
repeat (var iMember = 0; iMember < memberAmount; iMember++)
{
    byte unknown_98 // FF
    uint memberId
    ascii resultName
    uint objectId
    byte level
    byte healthMana // 0-A|0-A -> 0%-100%|0%-100%
    ushort region
    if (!position.Region.IsDungeon)
    {
        short xOffset
        short zOffset
        short yOffset
    }
    if (!(!position.Region.IsDungeon))
    {
        int xOffset
        int zOffset
        int yOffset
    }
    short worldId
    short layerId
    ascii guild
    byte unknown_107 // 04
    uint masteryId1
    uint masteryId2
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/GameClientType.cs
enum GameClientType : byte
{
    jSRO = 0,
    Japanese_Old = 1,
    Thailand = 2,
    Vietnam = 3,
    Taiwan_Old = 4,
    Vietnam193 = 5,
    Vietnam274 = 6,
    Chinese_Old = 7,
    Chinese = 8,
    Global = 9,
    Turkey = 10,
    VTC_Game = 11,
    Taiwan = 12,
    Korean = 13,
    Japanese = 14,
    RuSro = 15,
    Rigid = 16,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3065)]
    public sealed class PartyCreateFromMatching : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3065 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
