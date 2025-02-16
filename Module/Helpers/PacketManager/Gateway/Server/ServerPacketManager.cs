using Module.Engine.Classes;
using Module.Framework;
using Module.Helpers.PacketManager.Gateway.Server.Handlers;
using Module.Services;
using SilkroadSecurityAPI;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Client;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Server;

namespace Module.Helpers.PacketManager.Gateway.Server
{
    public class GatewayServerPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            if(client != null)
            {

            }

            return packet.Opcode switch
            {
                LOGIN_SERVER_HANDSHAKE or ACCEPT_HANDSHAKE => new Handshake(),
                SERVER_GATEWAY_LOGIN_RESPONSE => new LoginResponse(),
                SERVER_GATEWAY_SHARD_LIST_RESPONSE => new ShardListResponse(),
                SERVER_GATEWAY_PATCH_RESPONSE => new PatchResponse(),
                SERVER_GATEWAY_LOGIN_IBUV_CHALLENGE => new LoginIBUVChallange(),
                _ => null!,//Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
            };
        }
    }
}
