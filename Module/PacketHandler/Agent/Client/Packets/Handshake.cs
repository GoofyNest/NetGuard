using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public class Handshake : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new()
            {
                SkipSending = false,
                ResultType = PacketResultType.SkipSending
            };

            return response;
        }
    }
}
