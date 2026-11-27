/*
0x3047 | Agent | Server -> Client
Name: InventoryStorageDataBeginResponse

// Variant: InventoryStorageDataBeginResponse.Invoke
ulong gold

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3047)]
    public sealed class InventoryStorageDataBeginResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3047 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
