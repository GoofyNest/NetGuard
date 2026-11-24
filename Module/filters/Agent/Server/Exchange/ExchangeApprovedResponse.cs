/*
0x3087 | Agent | Server -> Client
Name: ExchangeApprovedResponse

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3087)]
    public sealed class ExchangeApprovedResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                NetGuard.Hot.InventoryTracking.Apply(packet, client);
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3087 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
