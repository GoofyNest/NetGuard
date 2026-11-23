using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3085)]
    public sealed class ExchangeWindow : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            client.Agent.InExchange = true;

            return response;
        }
    }
}