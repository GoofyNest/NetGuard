/*
0x3092 | Agent | Server -> Client
Name: InventoryUpdateSizeResponse

// Variant: InventoryUpdateSizeResponse.Invoke
byte type
byte size

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3092)]
    public sealed class InventoryUpdateSizeResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3092 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
