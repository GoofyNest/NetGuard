/*
0x3052 | Agent | Server -> Client
Name: InventoryUpdateDurabilityResponse

// Variant: InventoryUpdateDurabilityResponse.Invoke
byte slot
uint durability
if (item == null)
{
    // Stop parsing this variant.
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3052)]
    public sealed class InventoryUpdateDurabilityResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3052 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
