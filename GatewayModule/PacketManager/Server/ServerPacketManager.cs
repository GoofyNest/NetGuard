using System;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.PacketManager.Server.Handlers;
using SilkroadSecurityAPI;
using static GatewayModule.Framework.Opcodes.Client;
using static GatewayModule.Framework.Opcodes.Server;

namespace GatewayModule.PacketManager.Server
{
    public class ServerPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch (packet.Opcode)
            {
                case LOGIN_SERVER_HANDSHAKE:
                case CLIENT_ACCEPT_HANDSHAKE:
                    return new Handshake();

                case SERVER_GATEWAY_LOGIN_RESPONSE:
                    return new LoginResponse();

                default:
                    //Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                    return null;
            }
        }
    }
}
