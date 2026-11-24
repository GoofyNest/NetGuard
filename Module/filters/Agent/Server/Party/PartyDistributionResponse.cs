/*
0x3068 | Agent | Server -> Client
Name: PartyDistributionResponse

// Variant: PartyDistributionResponse.Invoke
uint partyMemberJID
if (member == null)
{
    // Stop parsing this variant.
}
uint itemId
if (item.TypeID1 == 3)
{
    if (item.TypeID2 == 1)
    {
        byte optLevel
    }
    if (!(item.TypeID2 == 1))
    {
        if (!(item.TypeID2 == 2))
        {
            if (item.TypeID2 == 3)
            {
                ushort quantity
            }
        }
    }
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3068)]
    public sealed class PartyDistributionResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3068 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
