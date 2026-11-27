/*
0x3085 | Agent | Server -> Client
Name: ExchangeStartedResponse

// Variant: ExchangeStartedResponse.Invoke
uint playerUniqueId

*/

using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3085)]
    public sealed class ExchangeStartedResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {

                var response = new PacketHandlingResult();

                client.Agent.InExchange = true;

                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3085 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}