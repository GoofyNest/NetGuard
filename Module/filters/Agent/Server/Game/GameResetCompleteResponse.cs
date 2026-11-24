/*
0x34B5 | Agent | Server -> Client
Name: GameResetCompleteResponse

// Variant: GameResetRequest.Invoke
if (Game.Player.Teleportation == null)
{
    // Stop parsing this variant.
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
    [Filter(0x34B5)]
    public sealed class GameResetCompleteResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x34B5 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
