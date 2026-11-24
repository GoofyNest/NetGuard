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
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x34A9 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
