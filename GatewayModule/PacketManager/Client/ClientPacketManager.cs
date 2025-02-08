using System;
using GatewayModule.PacketManager.Client.Handlers;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using PacketManager.Server.Handlers;
using SilkroadSecurityAPI;
using static Framework.Opcodes.Client;
using static Framework.Opcodes.Server;

namespace Framework
{
    public class ClientPacketManager
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            switch(packet.Opcode)
            {
                case LOGIN_SERVER_HANDSHAKE:
                case CLIENT_ACCEPT_HANDSHAKE:
                    return new ClientHandshakeHandler();

                case CLIENT_GATEWAY_PATCH_REQUEST:
                    return new ClientGatewayPatchRequestHandler();

                case CLIENT_GATEWAY_SHARD_LIST_REQUEST:
                    return new ClientGatewayShardListRequestHandler();

                case CLIENT_GATEWAY_LOGIN_REQUEST:
                    return new ClientGatewayLoginRequestHandler();

                case CLIENT_GATEWAY_NOTICE_REQUEST:
                    return new ClientGatewayNoticeRequest();

                case CLIENT_GATEWAY_SHARD_LIST_PING_REQUEST:
                    return new ClientGatewayShardListPingRequestHandler();

                case CLIENT_GATEWAY_LOGIN_IBUV_ANSWER:
                    return new ClientGatewayLoginIBUVAnswer();

                case GLOBAL_IDENTIFICATION:
                    return new ClientGlobalIdentification();

                case CLIENT_GLOBAL_PING:
                    return new ClientGlobalPing();

                default:
                    return new _ClientDefaultHandler();
            }
        }
    }
}
