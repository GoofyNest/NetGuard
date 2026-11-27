/*
0x3016 | Agent | Server -> Client
Name: EntitySingleDespawnResponse

// Variant: EntitySingleDespawnResponse.Invoke
uint uniqueId

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3016)]
    public sealed class EntitySingleDespawnResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3016 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
