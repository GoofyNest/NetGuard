namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA101)] // SERVER_GATEWAY_SHARD_LIST_RESPONSE
    public sealed class ShardListResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            client.Gateway.SentShardListResponse = true;
            return new PacketHandlingResult();
        }
    }
}