using Module.Engine.Classes;
using Module.Framework;
using Module.Helpers.PacketManager.Agent.Client.Handlers;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client
{
    public class AgentClientPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            if (Packets.badOpcodes.Contains(packet.Opcode))
            {
                return new DefaultBlock(); // Block the exploit
            }

            if (client.shardSettings.inCharSelection) // https://www.elitepvpers.com/forum/sro-pserver-guides-releases/4232366-release-disconnect-players-exploit-found-iwa.html
            {
                Custom.WriteLine($"client.inCharSelection working {client.shardSettings.inCharSelection}", ConsoleColor.DarkMagenta);

                switch (packet.Opcode)
                {
                    case (ushort)Global.Ping:
                    case (ushort)CharScreen.SelectionRename:
                    case (ushort)CharScreen.SelectionJoin:
                    case (ushort)CharScreen.SelectionAction:
                        {
                            // These packets are allowed in character selection.
                            if (packet.Opcode == (ushort)CharScreen.SelectionJoin)
                            {
                                if (!client.playerInfo.sentJoinRequest)
                                    client.playerInfo.currentChar = packet.ReadAscii(); // Temp storing char name, check CharacterJoin.cs in Server handler
                            }

                            // Do nothing, allow this
                            return null!;
                        }
                }

                // Other packets are dropped in character selection
                return new DefaultBlock();
            }

            if(!Packets.goodOpcodes.Contains(packet.Opcode))
            {
                Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.DarkMagenta);
            }

            switch (packet.Opcode)
            {
                // Block these packets whilst playing (not normal behaviour)
                case (ushort)CharScreen.SelectionRename:
                case (ushort)CharScreen.SelectionJoin:
                case (ushort)CharScreen.SelectionAction:
                    {
                        if (!client.shardSettings.inCharSelection)
                            return new DefaultBlock();

                        return null!;
                    }

                case (ushort)Skill.MasteryLearn:
                    return new SkillMasteryLearn();

                case (ushort)Character.InfoUpdate:
                    return new PlayerBerserk();

                case (ushort)MagicOption.Grant:
                    return new MagicOptionGrant();

                case (ushort)Guild.UpdateNotice:
                    return new GuildUpdateNotice();

                case (ushort)Siege.Action:
                    return new FortressSiegeAction();

                case (ushort)Global.AcceptHandshake:
                case (ushort)Global.HandShake:
                    return new Handshake();

                case (ushort)Game.Ready2:
                    return new SpoofLocaleVersion();

                case (ushort)Logout.Logout:
                    return new LogoutRequest();

                case (ushort)Global.Identification:
                    return new Global_Identification();

                case (ushort)Authentication.Auth:
                    return new AuthRequest();

                case (ushort)Config.Update:
                    return new ConfigUpdate();

                default:
                    //Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.DarkMagenta);
                    return null!;
            }
        }
    }
}
