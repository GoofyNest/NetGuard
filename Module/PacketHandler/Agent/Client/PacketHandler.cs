﻿using Module.Config;
using Module.Networking;
using Module.PacketHandler.Agent.Client.Packets;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            /*
                New way to block exploits

                This code will force people to login to send the rest of the Client -> Server packets
            */
            if (client.Agent.IsIngame == false)
            {
                /*
                    Block all known / unknown SR_ShardManager exploits
                    https://www.elitepvpers.com/forum/sro-pserver-guides-releases/4232366-release-disconnect-players-exploit-found-iwa.html

                    This code will limit people from sending unknown packets whilst in the Char selection screen
                */
                if (client.Agent.InCharSelectionScreen)
                {
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
                                    if (!client.Agent.SentJoinRequest)
                                        client.PlayerInfo.CurrentCharName = packet.ReadAscii(); // Temp storing char name, check CharacterJoin.cs in Server handler
                                }

                                // Do nothing, allow this
                                return null!;
                            }

                    }

                    // Other packets are dropped in character selection
                    Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending unknown packets whilst in char selection", ConsoleColor.Yellow);
                    return new DefaultBlock();
                }

                switch (packet.Opcode)
                {
                    case (ushort)Authentication.Auth:
                        return new AuthRequest();

                    case (ushort)Global.Identification:
                        return new Global_Identification();

                    case (ushort)Global.AcceptHandshake:
                    case (ushort)Global.HandShake:
                        return new Handshake();
                }

                Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending inGame packets whilst offline", ConsoleColor.Yellow);
                return new DefaultBlock();
            }

            if (PacketType.BadOpcodes.Contains(packet.Opcode))
            {
                return new DefaultBlock(); // Block the exploit
            }

            if (!PacketType.GoodOpcodes.Contains(packet.Opcode))
            {
                Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Magenta);
            }

            switch (packet.Opcode)
            {
                // Block these packets whilst playing (not normal behaviour)
                case (ushort)Authentication.Auth:
                case (ushort)Global.Identification:
                case (ushort)Global.AcceptHandshake:
                case (ushort)Global.HandShake:
                    {
                        if (client.Agent.InCharSelectionScreen)
                        {
                            Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending login packets whilst in char screen", ConsoleColor.Yellow);
                            return new DefaultBlock();
                        }

                        if (client.Agent.IsIngame)
                        {
                            Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending login packets whilst in-game", ConsoleColor.Yellow);
                            return new DefaultBlock();
                        }
                        return null!;
                    }

                // Block these packets whilst playing (not normal behaviour)
                case (ushort)CharScreen.SelectionRename:
                case (ushort)CharScreen.SelectionJoin:
                case (ushort)CharScreen.SelectionAction:
                    {
                        if (!client.Agent.InCharSelectionScreen)
                        {
                            Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending char screen packets when not logged in", ConsoleColor.Yellow);
                            return new DefaultBlock();
                        }

                        if (client.Agent.IsIngame)
                        {
                            Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending char screen packets when in-game", ConsoleColor.Yellow);
                            return new DefaultBlock();
                        }
                        return null!;
                    }

                case (ushort)Operator.Command:
                    return new GMCommand();

                case (ushort)Skill.MasteryLearn:
                    return new SkillMasteryLearn();

                case (ushort)Character.InfoUpdate:
                    return new PlayerBerserk();

                case (ushort)MagicOption.Grant:
                    return new MagicOptionGrant();

                case (ushort)Chat.Chat:
                    return new ChatMessage();

                case (ushort)Guild.UpdateNotice:
                    return new GuildUpdateNotice();

                case (ushort)Siege.Action:
                    return new FortressSiegeAction();

                case (ushort)Game.Ready2:
                    return new SpoofLocaleVersion();

                case (ushort)Logout.Logout:
                    return new LogoutRequest();

                case (ushort)Config.Update:
                    return new ConfigUpdate();

                default:
                    //Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Magenta);
                    return null!;
            }
        }
    }
}
