using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Client.Packets
{
    public class LoginIBUVAnswer : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            //string code = packet.ReadAscii();

            return response;
        }
    }
}
