using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Server.Packets
{
    public class Handshake : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new()
            {
                SkipSending = true,
                ResultType = PacketResultType.SkipSending
            };

            return response;
        }
    }
}
