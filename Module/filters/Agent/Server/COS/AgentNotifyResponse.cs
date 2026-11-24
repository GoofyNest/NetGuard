/*
0x300C | Agent | Server -> Client
Name: AgentNotifyResponse

// Variant: AgentNotifyResponse.Invoke
byte noticeType
if (gameVersion > Thailand)
{
    byte unknown_35
}
switch (noticeType): case 0x5:
{
    uint refObjId
    if (!Game.ReferenceManager.CharacterData.TryGetValue(refObjId, out var obj))
    {
        // Stop parsing this variant.
    }
}
switch (noticeType): case 0x6:
{
    uint refObjId
    if (!Game.ReferenceManager.CharacterData.TryGetValue(refObjId, out obj))
    {
        // Stop parsing this variant.
    }
    ascii characterName
    if (characterName == "???")
    {
        // Stop parsing this variant.
    }
}

// Variant: AgentNotifyResponse.Invoke
byte noticeType
if (gameVersion > Thailand)
{
    byte unknown_16
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
    [Filter(0x300C)]
    public sealed class AgentNotifyResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x300C handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
