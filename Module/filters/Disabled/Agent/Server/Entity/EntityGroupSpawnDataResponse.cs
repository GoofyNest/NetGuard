/*
0x3019 | Agent | Server -> Client
Name: EntityGroupSpawnDataResponse

// Variant: EntityGroupSpawnDataResponse.Invoke
if (Game.SpawnInfo == null)
{
    // Stop parsing this variant.
}

// Variant: EntityGroupSpawnBeginResponse.Invoke
// No payload fields identified at this source site.

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3019)]
    public sealed class EntityGroupSpawnDataResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3019 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
