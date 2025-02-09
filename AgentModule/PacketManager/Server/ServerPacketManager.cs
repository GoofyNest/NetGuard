using AgentModule.Engine.Classes;
using AgentModule.Framework;
using AgentModule.PacketManager.Server.Handlers;
using SilkroadSecurityAPI;
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
                case AGENT_CHARDATA_BEGIN:
                    return new CharDataLoadingStart();

                case AGENT_CHARDATA:
                    return new CharData();

                case LOGIN_SERVER_HANDSHAKE:
                case GLOBAL_PING:
                    return new Handshake();

                case AGENT_CHARDATA_END:
                    return new CharDataLoadingEnd();

                case AGENT_ENVIROMMENT_CELESTIAL_POSITION:
                    return new EnviromentCelestialPosition();

                case AGENT_CHARACTER_SELECTION_RESPONSE:
                    return new CharacterSelectionResponse();

                case AGENT_AUTH_RESPONSE:
                    return new AuthResponse();

                default:
                    //Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                    return null;
            }
        }
    }
}
