/*
0x3017 | Agent | Server -> Client
Name: EntityGroupSpawnBeginResponse

// Variant: EntityGroupSpawnBeginResponse.Invoke
byte type
ushort amount

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3017)]
    public sealed class EntityGroupSpawnBeginResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                NetGuard.Hot.InventoryNpcTracking.Apply(packet, client);
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3017 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
