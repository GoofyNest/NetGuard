/*
0x3054 | Agent | Server -> Client
Name: EntityAnimationPromoteResponse

// Variant: EntityAnimationPromoteResponse.Invoke
uint uniqueId

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3054)]
    public sealed class EntityAnimationPromoteResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
