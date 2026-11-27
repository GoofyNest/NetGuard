using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client, SessionContext ctx)
        {
            // 1. Landing — full, hot-reloadable control over every packet.
            var landing = FilterRegistry.RunLanding(FilterContext.AgentClient, packet, client, ctx);
            if (FilterRegistry.StopsDispatch(landing))
                return landing;

            // 2. Per-opcode filter.
            var filter = FilterRegistry.Resolve(FilterContext.AgentClient, packet.Opcode);
            return filter != null ? DynamicFilterAdapter.For(filter)
                : landing != null ? landing : null!;
        }
    }
}
