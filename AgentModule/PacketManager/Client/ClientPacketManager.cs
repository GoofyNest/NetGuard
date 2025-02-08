using NetGuard.Engine.Classes;
using SilkroadSecurityAPI;
using static Framework.Opcodes.Client;
using static Framework.Opcodes.Server;

namespace Framework
{
    public class ClientPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch(packet.Opcode)
            {
                default:
                    return null;
            }
        }
    }
}
