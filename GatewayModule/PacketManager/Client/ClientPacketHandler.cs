using PacketManager.Server.Handlers;
using SilkroadSecurityAPI;
using static Framework.Opcodes.Client;
using static Framework.Opcodes.Server;

namespace Framework
{
    public class ClientPacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet)
        {
            switch(packet.Opcode)
            {
                default:
                    return null;
            }
        }
    }
}
