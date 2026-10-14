using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Server
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            var dynamicFilter = FilterRegistry.Resolve(FilterContext.GatewayServer, packet.Opcode);
            if (dynamicFilter != null)
                return new DynamicFilterAdapter(dynamicFilter);

            // If you had a Default fallback, keep it here
            return null!; // or null! if you don't have one
        }
    }
}
