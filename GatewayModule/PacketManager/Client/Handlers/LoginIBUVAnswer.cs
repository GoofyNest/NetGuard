using GatewayModule.SilkroadSecurityAPI;
using GatewayModule.Framework;
using GatewayModule.Engine.Classes;

namespace GatewayModule.PacketManager.Client.Handlers
{
    public class LoginIBUVAnswer : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            string code = packet.ReadAscii();

            return response;
        }
    }
}
