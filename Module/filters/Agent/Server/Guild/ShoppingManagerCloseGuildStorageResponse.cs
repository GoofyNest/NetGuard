/*
0xB251 | Agent | Server -> Client
Name: ShoppingManagerCloseGuildStorageResponse

// Payload fields are not identified by the available source.

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
    [Filter(0xB251)]
    public sealed class ShoppingManagerCloseGuildStorageResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB251 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
