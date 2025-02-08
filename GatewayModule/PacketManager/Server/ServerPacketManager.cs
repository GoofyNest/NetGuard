using System;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using PacketManager.Server.Handlers;
using SilkroadSecurityAPI;
using static Framework.Opcodes.Client;
using static Framework.Opcodes.Server;

namespace Framework
{
    public class ServerPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch (packet.Opcode)
            {
                case LOGIN_SERVER_HANDSHAKE:
                case CLIENT_ACCEPT_HANDSHAKE:
                    return new ServerHandshakeHandler();

                case SERVER_GATEWAY_LOGIN_RESPONSE:
                    return new ServerLoginResponseHandler();

                default:
                    //Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                    return null;
            }
        }
    }
}
