/*
0x2116 | Gateway | Server -> Client
Name: GlobalGatewayLoginAccepted

// Variant: GlobalGatewayLoginAccepted.Invoke
byte isAccepted

*/

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0x2116)]
    public sealed class GlobalGatewayLoginAccepted : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x2116 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
