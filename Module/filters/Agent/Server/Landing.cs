using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;
using NetGuard.Hot;

namespace Module.PacketHandler.Filtering.Agent.Server
{
    /// <summary>
    /// Agent S->C landing. Runs before [Filter] dispatch for every packet.
    /// Current policy is forward-by-default, so returning null here preserves
    /// that behaviour. Add rules only when you actually need them:
    ///
    ///   - state tracking (e.g. remember login has started)
    ///   - response ordering (e.g. block a second shard list)
    ///   - logging / side effects
    ///   - pre-filter mutation
    ///
    /// Return a PacketHandlingResult to short-circuit (block / disconnect / etc).
    /// Return null to forward to the [Filter] layer, then default forward.
    /// </summary>
    public sealed class Landing : ILanding
    {
        public PacketHandlingResult? Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var agent = client.Host()!.Agent;
                if (packet.Opcode == 0x34B5)
                {
                    agent.IsTeleporting = true;
                    Custom.WriteLine($"[Teleport] START opcode=0x34B5 character={client.PlayerInfo.CurrentCharName}");
                }
                else if (agent.IsTeleporting && packet.Opcode is 0x3012 or 0x3077)
                {
                    agent.IsTeleporting = false;
                    Custom.WriteLine($"[Teleport] END opcode=0x{packet.Opcode:X4} character={client.PlayerInfo.CurrentCharName}");
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[Teleport] S->C opcode=0x{packet.Opcode:X4} tracking error: {ex.Message}", ConsoleColor.Red);
            }
            return null;
        }
    }
}
