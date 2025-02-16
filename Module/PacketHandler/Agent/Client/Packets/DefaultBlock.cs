using SilkroadSecurityAPI;
using Module.Networking;
using Module.PacketHandler;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public class DefaultBlock : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new()
            {
                ResultType = PacketResultType.Block
            };

            return response;
        }
    }
}
