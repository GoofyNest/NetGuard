/*
0xB202 | Agent | Server -> Client
Name: SkillWithdrawResponse

// Variant: SkillWithdrawResponse.Invoke
byte unknown_30
if (unknown_30 != 1)
{
    // Stop parsing this variant.
}
uint skillId // the new skill's id
if (oldSkill == null)
{
    // Stop parsing this variant.
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB202)]
    public sealed class SkillWithdrawResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB202 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
