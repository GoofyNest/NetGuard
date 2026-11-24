/*
0x706C | Agent | Client -> Server
Name: PartyMatchingListRequest

// Variant: PartyMatchingBundle.RequestPartyList
byte page

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

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x706C)]
    public sealed class PartyMatchingListRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
