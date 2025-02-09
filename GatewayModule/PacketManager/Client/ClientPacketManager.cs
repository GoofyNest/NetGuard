using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.PacketManager.Client.Handlers;
using SilkroadSecurityAPI;
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
                case ACCEPT_HANDSHAKE:
                    return new Handshake();

                case GATEWAY_PATCH_REQUEST:
                    return new PatchRequest();

                case GATEWAY_SHARD_LIST_REQUEST:
                    return new ShardListRequest();

                case GATEWAY_LOGIN_REQUEST:
                    return new LoginRequest();

                case GATEWAY_NOTICE_REQUEST:
                    return new NoticeRequest();

                case GATEWAY_SHARD_LIST_PING_REQUEST:
                    return new ShardListPingRequest();

                case GATEWAY_LOGIN_IBUV_ANSWER:
                    return new LoginIBUVAnswer();

                case GLOBAL_IDENTIFICATION:
                    return new Global_Identification();

                case GLOBAL_PING:
                    return new GlobalPing();

                default:
                    return new Default();
            }
        }
    }
}
