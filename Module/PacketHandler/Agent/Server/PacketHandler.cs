using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client, SessionContext ctx)
        {
            var landing = FilterRegistry.RunLanding(FilterContext.AgentServer, packet, client, ctx);
            if (landing != null)
                return new StaticResultHandler(landing);

            var filter = FilterRegistry.Resolve(FilterContext.AgentServer, packet.Opcode);
            return filter != null ? new DynamicFilterAdapter(filter) : null!;
        }
    }
}