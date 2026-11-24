/*
0xB0A1 | Agent | Server -> Client
Name: SkillLearnResponse

// Variant: SkillLearnResponse.Invoke
byte result
if (result != 1)
{
    // Stop parsing this variant.
}
uint skillId

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB0A1)]
    public sealed class SkillLearnResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB0A1 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
