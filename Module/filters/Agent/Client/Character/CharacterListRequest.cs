/*
0x7007 | Agent | Client -> Server
Name: CharacterListRequest

// Variant: AgentLoginResponse.Invoke
if (flag == 0x01)
{
    byte unknown_40 // value: 0x02
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

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7007)]
    public sealed class CharacterListRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
