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
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3054 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
