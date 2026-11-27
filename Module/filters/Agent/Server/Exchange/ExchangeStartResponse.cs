/*
0xB081 | Agent | Server -> Client
Name: ExchangeStartResponse

// Variant: ExchangeStartResponse.Invoke
byte unknown_17
if (unknown_17 != 1)
{
    // Stop parsing this variant.
}
uint playerUniqueId

*/

using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB081)]
    public sealed class ExchangeStartResponse : IFilter
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
                Custom.WriteLine("[S->C] 0xB081 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}