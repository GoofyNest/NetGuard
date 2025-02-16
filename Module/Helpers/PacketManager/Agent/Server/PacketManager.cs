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
            if (client != null)
            {
                // You can add any logic for handling the client here
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
                _ => new LogAndReturnNull(), // Default case
            };
        }
    }

    public class LogAndReturnNull : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            // Default action, could log or ignore packet
            //Custom.WriteLine($"Unhandled packet: {packet.Opcode:X4}", ConsoleColor.Yellow);
            return new PacketHandlingResult { }; // Or another result type
        }
    }
}