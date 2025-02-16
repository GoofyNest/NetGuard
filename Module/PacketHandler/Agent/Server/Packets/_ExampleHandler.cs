using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server.Packets
{
    public class ExampleHandler : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            return response;
        }
    }
}
