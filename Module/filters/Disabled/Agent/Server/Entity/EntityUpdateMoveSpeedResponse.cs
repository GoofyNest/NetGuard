/*
0x30D0 | Agent | Server -> Client
Name: EntityUpdateMoveSpeedResponse

// Variant: EntityUpdateMoveSpeedResponse.Invoke
uint uniqueId
float walkSpeed
float runSpeed
if (!(uniqueId == Game.Player.UniqueId || Game.Player.Vehicle?.UniqueId == uniqueId))
{
    if (!SpawnManager.TryGetEntity<SpawnedBionic>(uniqueId, out var bionic))
    {
        // Stop parsing this variant.
    }
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30D0)]
    public sealed class EntityUpdateMoveSpeedResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
