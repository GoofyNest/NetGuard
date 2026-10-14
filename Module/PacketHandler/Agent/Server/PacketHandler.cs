using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            // Dynamic filters first
            var dynamicFilter = FilterRegistry.Resolve(FilterContext.AgentServer, packet.Opcode);
            if (dynamicFilter != null)
                return new DynamicFilterAdapter(dynamicFilter);

            return null!;
        }
    }

    public class LogAndReturnNull : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            // Default action, could log or ignore packet
            //Custom.WriteLine($"Unhandled packet: {packet.Opcode:X4}", ConsoleColor.Yellow);
            return new PacketHandlingResult { }; // Or another result type
        }
    }
}