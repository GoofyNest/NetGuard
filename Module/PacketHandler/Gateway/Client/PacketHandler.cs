using Module.Networking;
using Module.PacketHandler.Gateway.Client.Packets;
using SilkroadSecurityAPI;
using static Module.PacketHandler.Gateway.Opcodes.Client;
using static Module.PacketHandler.Gateway.Opcodes.Server;

namespace Module.PacketHandler.Gateway.Client
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            if (client != null)
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
