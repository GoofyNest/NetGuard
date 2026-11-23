using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB084)]
    public sealed class ExchangeApprove : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            client.Agent.InExchange = false;

            return response;
        }
    }
}