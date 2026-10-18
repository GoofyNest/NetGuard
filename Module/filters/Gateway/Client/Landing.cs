using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering.Gateway.Client
{
    public sealed class Landing : ILanding
    {
        public PacketHandlingResult? Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            // No allowlist here. The [Filter] set is the allowlist.
            // This method is for rules that don't fit an opcode filter:
            //
            //   - state machine (e.g. "login packet arriving after auth")
            //   - rate limits (e.g. "same opcode more than N/s")
            //   - logging / side effects
            //   - pre-filter mutation
            //
            // Return null to continue to filter dispatch / Default.
            // Return a PacketHandlingResult to short-circuit.

            return null;
        }
    }
}