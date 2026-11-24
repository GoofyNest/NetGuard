/*
0x304D | Agent | Server -> Client
Name: EntityRemoveOwnershipResponse

// Variant: EntityRemoveOwnershipResponse.Invoke
uint itemUniqueId
if (!SpawnManager.TryGetEntity<SpawnedItem>(itemUniqueId, out var entity))
{
    // Stop parsing this variant.
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x304D)]
    public sealed class EntityRemoveOwnershipResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x304D handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
