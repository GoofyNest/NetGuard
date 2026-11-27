/*
0xB023 | Agent | Server -> Client
Name: EntityUpdatePositionResponse

// Variant: EntityUpdatePositionResponse.Invoke
uint uniqueId
ushort region
float xOffset
float zOffset
float yOffset
short angle
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
    [Filter(0xB023)]
    public sealed class EntityUpdatePositionResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
