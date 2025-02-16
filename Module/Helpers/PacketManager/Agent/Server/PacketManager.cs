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
            if(client != null)
            {

            }

            return packet.Opcode switch
            {
                (ushort)Global.Ping or (ushort)Global.HandShake => new Handshake(),
                (ushort)Agent.CharacterDataBegin => new CharDataLoadingStart(),
                (ushort)Agent.CharacterData => new CharData(),
                (ushort)Agent.CharacterDataEnd => new CharDataLoadingEnd(),
                (ushort)Agent.CharacterJoin => new CharacterJoin(),
                (ushort)Agent.EnvironmentCelestialPosition => new EnviromentCelestialPosition(),
                (ushort)Login.CharacterSelectionResponse => new CharacterSelectionResponse(),
                (ushort)Login.AuthResponse => new AuthResponse(),
                (ushort)Agent.SilkUpdate => new SilkUpdate(),
                _ => null!,//Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
            };
        }
    }
}
