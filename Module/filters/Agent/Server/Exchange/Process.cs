using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB081)]
    public sealed class ExchangeProcess : IFilter
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