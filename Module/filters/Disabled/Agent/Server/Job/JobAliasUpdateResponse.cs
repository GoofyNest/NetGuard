/*
0xB0E3 | Agent | Server -> Client
Name: JobAliasUpdateResponse

// Variant: JobAliasUpdateResponse.Invoke
byte result
if (result != 1)
{
    // Stop parsing this variant.
}
byte unknown_34 // IsUpdate
ascii jobInformationName

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB0E3)]
    public sealed class JobAliasUpdateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB0E3 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
