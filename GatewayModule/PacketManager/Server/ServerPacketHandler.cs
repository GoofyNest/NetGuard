using NetGuard.Engine.Classes;
using PacketManager.Server.Handlers;
using SilkroadSecurityAPI;
using static Framework.Opcodes.Client;
using static Framework.Opcodes.Server;

namespace Framework
{
    public class ServerPacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch (packet.Opcode)
            {
                case SERVER_GATEWAY_LOGIN_RESPONSE:
                    return new ServerLoginResponseHandler();

                default:
                    return null;
            }
        }
    }
}
