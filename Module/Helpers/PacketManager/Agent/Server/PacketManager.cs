using Module.Engine.Classes;
using Module.Framework;
using Module.Helpers.PacketManager.Agent.Server.Handlers;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Server
{
    public class AgentServerPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch (packet.Opcode)
            {
                case (ushort)Global.Ping:
                case (ushort)Global.HandShake:
                    return new Handshake();

                case (ushort)Agent.CharacterDataBegin:
                    return new CharDataLoadingStart();

                case (ushort)Agent.CharacterData:
                    return new CharData();

                case (ushort)Agent.CharacterDataEnd:
                    return new CharDataLoadingEnd();

                case (ushort)Agent.CharacterJoin:
                    return new CharacterJoin();

                case (ushort)Agent.EnvironmentCelestialPosition:
                    return new EnviromentCelestialPosition();

                case (ushort)Login.CharacterSelectionResponse:
                    return new CharacterSelectionResponse();

                case (ushort)Login.AuthResponse:
                    return new AuthResponse();

                default:
                    //Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                    return null!;
            }
        }
    }
}
