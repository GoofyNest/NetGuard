/*
0x325F | Agent | Server -> Client
Name: ActionItemPerkAddResponse

// Variant: ActionItemPerkAddResponse.Invoke
uint targetId
uint refObjItemId
uint token
uint value
uint remainingTime
if (!SpawnManager.TryGetEntityIncludingMe(targetId, out var target))
{
    // Stop parsing this variant.
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x325F)]
    public sealed class ActionItemPerkAddResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x325F handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
