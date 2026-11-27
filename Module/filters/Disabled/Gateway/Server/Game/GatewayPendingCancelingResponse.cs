/*
0xA10F | Gateway | Server -> Client
Name: GatewayPendingCancelingResponse

// Variant: GatewayPendingCancelingResponse.Invoke
byte unknown_31

*/

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA10F)]
    public sealed class GatewayPendingCancelingResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xA10F handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
