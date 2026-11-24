/*
0x6101 | Gateway | Client -> Server
Name: GatewayServerListRequestHook

// Variant: ClientlessManager.RequestServerList
// No payload fields identified at this source site.

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x6101)]
    public sealed class GatewayServerListRequestHook : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            if (packet.GetBytes().Length > 0)
                response.ResultType = PacketResultType.Block;

            Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_REQUEST {packet.GetBytes().Length}", ConsoleColor.Magenta);

            return response;
        }
    }
}