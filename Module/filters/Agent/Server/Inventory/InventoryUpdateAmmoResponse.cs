/*
0x3201 | Agent | Server -> Client
Name: InventoryUpdateAmmoResponse

// Variant: InventoryUpdateAmmoResponse.Invoke
ushort ammunitionAmount

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3201)]
    public sealed class InventoryUpdateAmmoResponse : IFilter
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
                Custom.WriteLine("[S->C] 0x3201 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
