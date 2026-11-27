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
            return new PacketHandlingResult();
        }
    }
}
