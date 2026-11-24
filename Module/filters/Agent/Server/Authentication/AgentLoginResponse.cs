/*
0xA103 | Agent | Server -> Client
Name: AgentLoginResponse

// Variant: AgentLoginResponse.Invoke
byte flag
if (flag == 0x01)
{
    if (!Game.Clientless)
    {
        // Stop parsing this variant.
    }
    // Stop parsing this variant.
}
byte code

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
    [Filter(0xA103)]
    public sealed class AgentLoginResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var response = new PacketHandlingResult();
                byte errorCode = packet.ReadUInt8();
                if (errorCode == 0x01)
                    client.Agent.InCharSelectionScreen = true;
                Custom.WriteLine($"SERVER_AGENT_AUTH_RESPONSE {errorCode}", ConsoleColor.Cyan);
                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xA103 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}