using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x34A5)]
    public sealed class CharDataLoadingStart : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            client.Host()!.Agent.CharData = null;
            return new PacketHandlingResult();
        }
    }
}