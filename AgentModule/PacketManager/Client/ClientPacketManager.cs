using AgentModule.Engine.Classes;
using AgentModule.Framework;
using SilkroadSecurityAPI;
using static AgentModule.Framework.Opcodes.Client;
using static AgentModule.Framework.Opcodes.Server;

namespace AgentModule.PacketManager.Client
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
