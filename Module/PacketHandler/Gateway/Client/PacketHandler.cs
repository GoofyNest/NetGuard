using Module.Networking;
using Module.PacketHandler.Filtering;
using Module.PacketHandler.Gateway.Client.Packets;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Client
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client, SessionContext ctx)
        {
            // 1. Landing — state machine, cross-cutting rules, side effects.
            //    Empty results allow opcode dispatch; explicit actions stop it.
            var landing = FilterRegistry.RunLanding(FilterContext.GatewayClient, packet, client, ctx);
            if (FilterRegistry.StopsDispatch(landing))
                return new StaticResultHandler(landing);

            // 2. Opcode filter — if one exists, let it run.
            var filter = FilterRegistry.Resolve(FilterContext.GatewayClient, packet.Opcode);
            if (filter != null)
                return new DynamicFilterAdapter(filter);

            // 3. No filter registered → block (unchanged behavior).
            return landing != null ? new StaticResultHandler(landing) : new Default();
        }
    }
}
