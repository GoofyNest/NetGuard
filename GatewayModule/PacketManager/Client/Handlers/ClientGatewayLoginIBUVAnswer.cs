using Framework;
using NetGuard.Engine.Classes;
using SilkroadSecurityAPI;

namespace GatewayModule.PacketManager.Client.Handlers
{
    public class ClientGatewayLoginIBUVAnswer : IPacketHandler
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
