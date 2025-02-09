﻿using AgentModule.Engine.Classes;
using AgentModule.Framework;
using AgentModule.PacketManager.Client.Handlers;
using SilkroadSecurityAPI;
using static AgentModule.Framework.Opcodes.Client;
using static AgentModule.Framework.Opcodes.Server;

namespace AgentModule.PacketManager.Client
{
    public class ClientPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            if (client.inCharSelection)
            {
                switch (packet.Opcode)
                {
                    case GLOBAL_PING:
                    case AGENT_CHARACTER_SELECTION_ACTION_REQUEST:
                    case AGENT_CHARACTER_SELECTION_JOIN_REQUEST:
                        // These packets are allowed in character selection.
                        return null;
                }

                // Other packets are dropped in character selection
                return new DefaultBlock();
            }

            switch (packet.Opcode)
            {
                case LOGIN_SERVER_HANDSHAKE:
                case ACCEPT_HANDSHAKE:
                    return new Handshake();

                case AGENT_GAME_READY2:
                    return new SpoofLocaleVersion();

                case AGENT_LOGOUT_REQUEST:
                    return new LogoutRequest();

                case GLOBAL_IDENTIFICATION:
                    return new Global_Identification();

                case AGENT_AUTH_REQUEST:
                    return new AuthRequest();

                default:
                    //Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                    return null;
            }
        }
    }
}
