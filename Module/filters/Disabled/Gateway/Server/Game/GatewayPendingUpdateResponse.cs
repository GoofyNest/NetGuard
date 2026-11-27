/*
0x210E | Gateway | Server -> Client
Name: GatewayPendingUpdateResponse

// Variant: GatewayPendingUpdateResponse.Invoke
byte unknown_57
if (unknown_57 != 1)
{
    // Stop parsing this variant.
}
ushort end
int timestamp
ushort begin

*/

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0x210E)]
    public sealed class GatewayPendingUpdateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x210E handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
