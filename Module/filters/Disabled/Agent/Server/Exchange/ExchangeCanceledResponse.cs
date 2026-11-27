/*
0x3088 | Agent | Server -> Client
Name: ExchangeCanceledResponse

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3088)]
    public sealed class ExchangeCanceledResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3088 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
