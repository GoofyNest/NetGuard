using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Server
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client, SessionContext ctx)
        {
            var landing = FilterRegistry.RunLanding(FilterContext.GatewayServer, packet, client, ctx);
            if (FilterRegistry.StopsDispatch(landing))
                return new StaticResultHandler(landing);

            var filter = FilterRegistry.Resolve(FilterContext.GatewayServer, packet.Opcode);
            return filter != null ? new DynamicFilterAdapter(filter)
                : landing != null ? new StaticResultHandler(landing) : null!;
        }
    }
}
