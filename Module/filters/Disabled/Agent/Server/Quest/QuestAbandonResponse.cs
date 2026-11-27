/*
0xB0D9 | Agent | Server -> Client
Name: QuestAbandonResponse

// Variant: QuestAbandonResponse.Invoke
byte unknown_29
if (unknown_29 == 0x01)
{
    uint questId
    if (!Game.Player.QuestLog.ActiveQuests.TryGetValue(questId, out var playerQuest))
    {
        // Stop parsing this variant.
    }
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

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB0D9)]
    public sealed class QuestAbandonResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB0D9 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
