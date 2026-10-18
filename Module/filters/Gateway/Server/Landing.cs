using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering.Gateway.Server
{
    /// <summary>
    /// Gateway S->C landing. Runs before [Filter] dispatch for every packet.
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
            return null;
        }
    }
}