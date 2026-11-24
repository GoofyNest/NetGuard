/*
0x3028 | Agent | Server -> Client
Name: EntitySourcePositionUpdate

// Variant: EntitySourcePositionUpdate.Invoke
ushort region
float xOffset
float zOffset
float yOffset
short angle
uint uniqueId
if (uniqueId == Game.Player.UniqueId)
{
    // Stop parsing this variant.
}
if (!SpawnManager.TryGetEntity<SpawnedEntity>(uniqueId, out var entity))
{
    // Stop parsing this variant.
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3028)]
    public sealed class EntitySourcePositionUpdate : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3028 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
