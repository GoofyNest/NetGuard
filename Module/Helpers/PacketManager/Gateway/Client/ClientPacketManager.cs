using Module.Engine.Classes;
using Module.Framework;
using Module.Helpers.PacketManager.Gateway.Client.Handlers;
using SilkroadSecurityAPI;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Client;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Server;

namespace Module.Helpers.PacketManager.Gateway.Client
{
    public class GatewayClientPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            if(client != null)
            {

            }

            return packet.Opcode switch
            {
                LOGIN_SERVER_HANDSHAKE or ACCEPT_HANDSHAKE => new Handshake(),
                GATEWAY_PATCH_REQUEST => new PatchRequest(),
                GATEWAY_SHARD_LIST_REQUEST => new ShardListRequest(),
                GATEWAY_LOGIN_REQUEST => new LoginRequest(),
                GATEWAY_NOTICE_REQUEST => new NoticeRequest(),
                GATEWAY_SHARD_LIST_PING_REQUEST => new ShardListPingRequest(),
                GATEWAY_LOGIN_IBUV_ANSWER => new LoginIBUVAnswer(),
                GLOBAL_IDENTIFICATION => new Global_Identification(),
                GLOBAL_PING => new GlobalPing(),
                _ => new Default(),
            };
        }
    }
}
