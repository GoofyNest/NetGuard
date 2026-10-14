using Module.Networking;
using Module.PacketHandler.Filtering;
using Module.PacketHandler.Gateway.Client.Packets;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Client
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            // Dynamic filters first
            var dynamicFilter = FilterRegistry.Resolve(FilterContext.GatewayClient, packet.Opcode);
            if (dynamicFilter != null)
                return new DynamicFilterAdapter(dynamicFilter);

            // Fallback only — all opcode-specific handlers moved to filters/
            return new Default();
        }
    }
}
