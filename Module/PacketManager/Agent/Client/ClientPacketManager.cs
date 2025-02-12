using Module.Engine.Classes;
using Module.Framework;
using Module.PacketManager.Agent.Client.Handlers;
using Module.Services;
using SilkroadSecurityAPI;
using _Agent = Module.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.PacketManager.Agent.ClientPackets.Shard;

namespace Module.PacketManager.Agent.Client
{
    public class AgentClientPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            if (client.inCharSelection) // https://www.elitepvpers.com/forum/sro-pserver-guides-releases/4232366-release-disconnect-players-exploit-found-iwa.html
            {
                Custom.WriteLine($"client.inCharSelection working {client.inCharSelection}", ConsoleColor.DarkMagenta);

                switch (packet.Opcode)
                {
                    case (ushort)_Global.Ping:
                    case (ushort)_Shard.CharacterSelectionRenameRequest:
                    case (ushort)_Shard.CharacterSelectionJoinRequest:
                    case (ushort)_Shard.CharacterSelectionActionRequest:
                        // These packets are allowed in character selection.
                        return null!;
                }

                // Other packets are dropped in character selection
                return new DefaultBlock();
            }

            switch (packet.Opcode)
            {
                case (ushort)_Global.AcceptHandshake:
                case (ushort)_Global.HandShake:
                    return new Handshake();

                case (ushort)_Agent.GameReady2:
                    return new SpoofLocaleVersion();

                case (ushort)_Login.LogoutRequest:
                    return new LogoutRequest();

                case (ushort)_Global.Identification:
                    return new Global_Identification();

                case (ushort)_Login.AuthRequest:
                    return new AuthRequest();

                case (ushort)_Agent.ConfigUpdate:
                    return new ConfigUpdate();

                default:
                    Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.DarkMagenta);
                    return null!;
            }
        }
    }
}
