/*
0xA100 | Gateway | Server -> Client
Name: GatewayPatchResponse

// Variant: GatewayPatchResponse.Invoke
byte unknown_32
if (!(unknown_32 == 0x01))
{
    byte flag // Error code;
    if (flag == 2)
    {
        ascii unknown_45
        ushort unknown_46 // DLServer Port
        int version
    }
}

*/

using NetGuard.Hot;

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA100)]
    public sealed class GatewayPatchResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                client.Gateway.SentPatchResponse = true;
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xA100 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}