using SilkroadSecurityAPI;
using Module.Framework;
using Module.Engine.Classes;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Client;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Server;

namespace Module.Helpers.PacketManager.Gateway.Client.Handlers
{
    public class LoginIBUVAnswer : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            string code = packet.ReadAscii();

            return response;
        }
    }
}
