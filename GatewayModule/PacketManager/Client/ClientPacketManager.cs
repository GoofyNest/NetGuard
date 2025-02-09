using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.PacketManager.Client.Handlers;
using GatewayModule.SilkroadSecurityAPI;
using static GatewayModule.Framework.Opcodes.Client;
using static GatewayModule.Framework.Opcodes.Server;

namespace GatewayModule.PacketManager.Client
{
    public class ClientPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch(packet.Opcode)
            {
                case LOGIN_SERVER_HANDSHAKE:
                case CLIENT_ACCEPT_HANDSHAKE:
                    return new Handshake();

                case CLIENT_GATEWAY_PATCH_REQUEST:
                    return new PatchRequest();

                case CLIENT_GATEWAY_SHARD_LIST_REQUEST:
                    return new ShardListRequest();

                case CLIENT_GATEWAY_LOGIN_REQUEST:
                    return new LoginRequest();

                case CLIENT_GATEWAY_NOTICE_REQUEST:
                    return new NoticeRequest();

                case CLIENT_GATEWAY_SHARD_LIST_PING_REQUEST:
                    return new ShardListPingRequest();

                case CLIENT_GATEWAY_LOGIN_IBUV_ANSWER:
                    return new LoginIBUVAnswer();

                case GLOBAL_IDENTIFICATION:
                    return new Global_Identification();

                case CLIENT_GLOBAL_PING:
                    return new GlobalPing();

                default:
                    return new Default();
            }
        }
    }
}
