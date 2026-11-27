/*
0xA117 | Gateway | Server -> Client
Name: GlobalSecondaryPasswordResponse

// Variant: GlobalSecondaryPasswordResponse.Invoke
byte type
if (type != 4)
{
    // Stop parsing this variant.
}
byte result
if (result == 1)
{
    // Stop parsing this variant.
}
byte errorCode

*/

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA117)]
    public sealed class GlobalSecondaryPasswordResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xA117 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
