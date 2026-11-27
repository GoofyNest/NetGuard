/*
0xB024 | Agent | Server -> Client
Name: EntityUpdateAngleResponse

// Variant: EntityUpdateAngleResponse.Invoke
uint uniqueId
short angle
if (Game.Player.UniqueId == uniqueId)
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
    [Filter(0xB024)]
    public sealed class EntityUpdateAngleResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
