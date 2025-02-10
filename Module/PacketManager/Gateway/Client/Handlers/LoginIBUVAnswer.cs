using SilkroadSecurityAPI;
using Module.Framework;
using Module.Engine.Classes;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Client.Handlers
{
    public class LoginIBUVAnswer : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            string code = packet.ReadAscii();

            return response;
        }
    }
}
