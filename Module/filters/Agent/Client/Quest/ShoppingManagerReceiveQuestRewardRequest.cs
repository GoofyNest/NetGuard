/*
0x7515 | Agent | Client -> Server
Name: ShoppingManagerReceiveQuestRewardRequest

// Variant: ShoppingManager.ReceiveQuestReward
uint questId
byte unknown_377 // value: 1
uint rewardId

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
    [Filter(0x7515)]
    public sealed class ShoppingManagerReceiveQuestRewardRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
