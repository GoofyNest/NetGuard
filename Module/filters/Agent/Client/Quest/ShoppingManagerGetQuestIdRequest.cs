/*
0x30D4 | Agent | Client -> Server
Name: ShoppingManagerGetQuestIdRequest

// Variant: ShoppingManager.GetQuestId
byte unknown_352 // value: 5

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

// Library/RSBot.Core/Objects/TalkOption.cs
enum TalkOption : byte
{
    Store = 1,
    Quest = 2,
    Storage = 3,
    Repair = 4,
    Monster = 5,
    SetReturnPoint = 7,
    TeleportTarget = 8,
    TeleportReturnPoint = 9,
    WithdrawSkill = 10,
    Stable = 11,
    Trade = 12,
    Guild = 15,
    MagicPopPlay = 17,
    MagicPopExchange = 18,
    EventChristmasExchange = 19,
    Trader = 20,
    Tief = 21,
    Hunter = 22,
    FortressAdministration = 23,
    FortressApplication = 24,
    FortressStructManagement = 25,
    FortressItemProduction = 26,
    FortressTraining = 27,
    TeleportLastLocation = 28,
    TeleportGuide = 30,
    FortressPully = 31,
    GrantMagicOption = 32,
    ArenaManager = 33,
    ArenaItemManager = 34,
    Consigment = 35,
    SummonPartyMember = 39,
    TeleportExitFortressDungeon = 40,
    TeleportExitDungeon = 41,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x30D4)]
    public sealed class ShoppingManagerGetQuestIdRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
