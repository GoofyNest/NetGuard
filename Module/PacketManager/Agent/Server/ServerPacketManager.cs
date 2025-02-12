using Module.Engine.Classes;
using Module.Framework;
using Module.PacketManager.Agent.Server.Handlers;
using SilkroadSecurityAPI;

using _Agent = Module.PacketManager.Agent.ServerPackets.Agent;
using _Global = Module.PacketManager.Agent.ServerPackets.Global;
using _Login = Module.PacketManager.Agent.ServerPackets.Login;

namespace Module.PacketManager.Agent.Server
{
    public class AgentServerPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch (packet.Opcode)
            {
                case (ushort)_Global.Ping:
                case (ushort)_Global.HandShake:
                    return new Handshake();

                case (ushort)_Agent.CharacterDataBegin:
                    return new CharDataLoadingStart();

                case (ushort)_Agent.CharacterData:
                    return new CharData();

                case (ushort)_Agent.CharacterDataEnd:
                    return new CharDataLoadingEnd();

                case (ushort)_Agent.EnvironmentCelestialPosition:
                    return new EnviromentCelestialPosition();

                case (ushort)_Login.CharacterSelectionResponse:
                    return new CharacterSelectionResponse();

                case (ushort)_Login.AuthResponse:
                    return new AuthResponse();

                default:
                    //Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                    return null!;
            }
        }
    }
}
