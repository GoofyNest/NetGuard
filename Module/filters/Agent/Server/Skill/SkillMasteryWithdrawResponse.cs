/*
0xB203 | Agent | Server -> Client
Name: SkillMasteryWithdrawResponse

// Variant: SkillMasteryWithdrawResponse.Invoke
byte unknown_29
if (unknown_29 != 0x01)
{
    // Stop parsing this variant.
}
uint masteryId
byte level

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB203)]
    public sealed class SkillMasteryWithdrawResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB203 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
