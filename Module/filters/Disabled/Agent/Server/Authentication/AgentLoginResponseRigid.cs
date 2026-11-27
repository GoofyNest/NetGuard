/*
0xA118 | Agent | Server -> Client
Name: AgentLoginResponseRigid

// Variant: AgentLoginResponseRigid.Invoke
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
    [Filter(0xA118)]
    public sealed class AgentLoginResponseRigid : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
