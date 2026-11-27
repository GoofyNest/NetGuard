/*
0xA323 | Gateway | Server -> Client
Name: CaptchaResponse

// Variant: CaptchaResponse.Invoke
byte flag
if (!(flag == 0x01))
{
    uint maxAttempts
    uint attempts
}

*/

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA323)]
    public sealed class CaptchaResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xA323 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
