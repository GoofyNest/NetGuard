/*
0x30E0 | Agent | Server -> Client
Name: JobUpdatePriceResponse

// Variant: JobUpdatePriceResponse.Invoke
byte itemCount
repeat (var i = 0; i < itemCount; i++)
{
    uint itemId
    uint sellPrice
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30E0)]
    public sealed class JobUpdatePriceResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x30E0 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
