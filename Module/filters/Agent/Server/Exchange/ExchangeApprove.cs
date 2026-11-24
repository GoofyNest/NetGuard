/*
0xB084 | Agent | Server -> Client
Name: ExchangeApprove

// Payload fields are not identified by the available source.

*/

using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB084)]
    public sealed class ExchangeApprove : IFilter
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
                Custom.WriteLine("[S->C] 0xB084 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}