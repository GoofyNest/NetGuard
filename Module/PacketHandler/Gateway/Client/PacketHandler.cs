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
            //    Return null to continue; return a result to short-circuit.
            var landing = FilterRegistry.RunLanding(FilterContext.GatewayClient, packet, client, ctx);
            if (landing != null)
                return new StaticResultHandler(landing);

            // 2. Opcode filter — if one exists, let it run.
            var filter = FilterRegistry.Resolve(FilterContext.GatewayClient, packet.Opcode);
            if (filter != null)
                return new DynamicFilterAdapter(filter);

            // 3. No filter registered → block (unchanged behavior).
            return new Default();
        }
    }
}