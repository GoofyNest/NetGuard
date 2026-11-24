/*
0xB083 | Agent | Server -> Client
Name: ExchangeClose

// Payload fields are not identified by the available source.

*/

using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB083)]
    public sealed class ExchangeClose : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var h = client.Host()!;
                var response = new PacketHandlingResult();
                client.Agent.InExchange = false;
                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB083 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}