/*
0x3253 | Agent | Server -> Client
Name: InventoryGuildStorageDataBeginResponse

// Variant: InventoryGuildStorageDataBeginResponse.Invoke
ulong gold

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3253)]
    public sealed class InventoryGuildStorageDataBeginResponse : IFilter
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
                Custom.WriteLine("[S->C] 0x3253 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
