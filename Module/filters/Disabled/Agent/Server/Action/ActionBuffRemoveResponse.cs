/*
0xB072 | Agent | Server -> Client
Name: ActionBuffRemoveResponse

// Variant: ActionBuffRemoveResponse.Invoke
byte buffTokensCount
repeat (var i = 0; i < buffTokensCount; i++)
{
    uint token
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB072)]
    public sealed class ActionBuffRemoveResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
