/*
0x3261 | Agent | Server -> Client
Name: ActionItemPerkRemoveResponse

// Variant: ActionItemPerkRemoveResponse.Invoke
uint targetId
uint unknown_15 // refObjItem.Id
uint token
if (!SpawnManager.TryGetEntityIncludingMe(targetId, out var target))
{
    // Stop parsing this variant.
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3261)]
    public sealed class ActionItemPerkRemoveResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
