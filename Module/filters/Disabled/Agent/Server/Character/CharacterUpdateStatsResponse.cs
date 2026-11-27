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
            return new PacketHandlingResult();
        }
    }
}
