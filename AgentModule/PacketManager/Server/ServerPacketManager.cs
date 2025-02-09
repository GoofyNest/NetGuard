using AgentModule.Engine.Classes;
using AgentModule.Framework;
using AgentModule.SilkroadSecurityAPI;
using static AgentModule.Framework.Opcodes.Client;
using static AgentModule.Framework.Opcodes.Server;

namespace AgentModule.PacketManager.Server
{
    public class ServerPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch (packet.Opcode)
            {

                default:
                    return null;
            }
        }
    }
}
