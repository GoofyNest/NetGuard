namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x6106)]
    public sealed class ShardListPingRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_PING_REQUEST {packet.GetBytes().Length}", ConsoleColor.Magenta);

            return response;
        }
    }
}