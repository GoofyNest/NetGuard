/*
0x3255 | Agent | Server -> Client
Name: InventoryGuildStorageDataResponse

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3255)]
    public sealed class InventoryGuildStorageDataResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3255 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
