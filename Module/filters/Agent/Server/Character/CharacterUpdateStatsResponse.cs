/*
0x303D | Agent | Server -> Client
Name: CharacterUpdateStatsResponse

// Variant: CharacterUpdateStatsResponse.Invoke
uint physicalAttackMin
uint physicalAttackMax
uint magicalAttackMin
uint magicalAttackMax
ushort physicalDefence
ushort magicalDefence
ushort hitRate
ushort parryRate
int maximumHealth
int maximumMana
ushort strength
ushort intelligence

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x303D)]
    public sealed class CharacterUpdateStatsResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x303D handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
