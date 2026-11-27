/*
0xB06B | Agent | Server -> Client
Name: PartyMatchingDeleteResponse

// Variant: PartyMatchingDeleteResponse.Invoke
byte unknown_27
if (unknown_27 != 0x01)
{
    // Stop parsing this variant.
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB06B)]
    public sealed class PartyMatchingDeleteResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB06B handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
