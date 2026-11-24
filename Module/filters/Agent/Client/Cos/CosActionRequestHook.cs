/*
0x705C | Agent | Client -> Server
Name: CosActionRequestHook

// Variant: CosActionRequestHook.ReplacePacket
uint uniqueId
byte type

*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x705C)]
    public sealed class CosActionRequestHook : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
