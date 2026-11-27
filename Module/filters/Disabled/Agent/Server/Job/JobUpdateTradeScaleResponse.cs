/*
0x30E8 | Agent | Server -> Client
Name: JobUpdateTradeScaleResponse

// Variant: JobUpdateTradeScaleResponse.Invoke
byte scale

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30E8)]
    public sealed class JobUpdateTradeScaleResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x30E8 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
