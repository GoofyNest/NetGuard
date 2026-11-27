/*
0x34A9 | Agent | Server -> Client
Name: MagicOptionGrantResponse

// Variant: MagicOptionGrantResponse.Invoke
byte slot
ascii attribute

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x34A9)]
    public sealed class MagicOptionGrantResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
