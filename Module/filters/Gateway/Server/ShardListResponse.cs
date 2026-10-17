using NetGuard.Hot;

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA101)]
    public sealed class ShardListResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            client.Gateway.SentShardListResponse = true;
            return new PacketHandlingResult();
        }
    }
}