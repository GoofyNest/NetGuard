/*
0xB06A | Agent | Server -> Client
Name: PartyMatchingChangeResponse

// Variant: PartyMatchingChangeResponse.Invoke
byte unknown_29
if (unknown_29 != 0x01)
{
    // Stop parsing this variant.
}
uint id
uint unknown_33
byte settings
byte purpose
byte levelFrom
byte levelTo
if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
{
    unicode title
}
else
{
    ascii title
}

// Response fields checked by PartyMatchingBundle.Change
byte predicate

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
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
    [Filter(0xB06A)]
    public sealed class PartyMatchingChangeResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB06A handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
