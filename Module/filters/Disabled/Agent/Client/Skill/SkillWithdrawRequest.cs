/*
0x7202 | Agent | Client -> Server
Name: SkillWithdrawRequest

// Variant: SkillWithdrawRequest.Invoke
uint unknown_29 // withdrawingItemId
uint skillId
byte level

*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7202)]
    public sealed class SkillWithdrawRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
