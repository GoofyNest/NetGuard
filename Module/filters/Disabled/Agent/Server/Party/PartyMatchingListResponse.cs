/*
0xB06C | Agent | Server -> Client
Name: PartyMatchingListResponse

// Response fields checked by PartyMatchingBundle.RequestPartyList
byte unknown_47
byte pageCount
byte page
byte partyCount
repeat (var i = 0; i < result.PartyCount; i++)
{
    uint id
    uint unknown_87 // leaderUniqueId
    if (gameVersion >= Chinese && gameVersion != Rigid)
    {
        uint unknown_90 // unknown
    }
    ascii leader
    byte race
    byte memberCount
    byte settings
    byte purpose
    byte minLevel
    byte maxLevel
    if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
    {
        unicode title
    }
    else
    {
        ascii title
    }
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
}

// Library/RSBot.Core/Client/ReferenceObjects/ObjectCountry.cs
enum ObjectCountry : byte
{
    Chinese = 0,
    Europe = 1,
    Islam = 2,
    Unassigned = 3,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}

// Library/RSBot.Core/Objects/Party/PartyPurpose.cs
enum PartyPurpose : byte
{
    Hunting = 0,
    Quest = 1,
    Trade = 2,
    Thief = 3,
}

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
    [Filter(0xB06C)]
    public sealed class PartyMatchingListResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB06C handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
