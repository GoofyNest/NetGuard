using System;
using Module.Engine.Classes;
using Module.Framework;
using Module.PacketManager.GatewayModule.Server.Handlers;
using SilkroadSecurityAPI;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Server
{
    public class GatewayServerPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch (packet.Opcode)
            {
                case LOGIN_SERVER_HANDSHAKE:
                case ACCEPT_HANDSHAKE:
                    return new Handshake();

                case SERVER_GATEWAY_LOGIN_RESPONSE:
                    return new LoginResponse();

                case SERVER_GATEWAY_SHARD_LIST_RESPONSE:
                    return new ShardListResponse();

                case SERVER_GATEWAY_PATCH_RESPONSE:
                    return new PatchResponse();

                default:
                    //Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                    return null!;
            }
        }
    }
}
