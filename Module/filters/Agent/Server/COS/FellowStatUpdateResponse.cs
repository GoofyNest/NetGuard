/*
0x3422 | Agent | Server -> Client
Name: FellowStatUpdateResponse

// Variant: FellowStatUpdateResponse.Invoke
uint uniqueId
if (fellow?.UniqueId != uniqueId)
{
    // Stop parsing this variant.
}
ushort strength
ushort intelligence
uint physicalAttackMin
uint physicalAttackMax
uint magicalAttackMin
uint magicalAttackMax
ushort physicalDefence
ushort magicalDefence
ushort unknown_43 // phy absorb
ushort unknown_44 // mag absorb
ushort hitRate
ushort unknown_47 // parry rate
ushort unknown_48 // critical
ushort unknown_49 // blocking rate
int maxHealth

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3422)]
    public sealed class FellowStatUpdateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3422 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
