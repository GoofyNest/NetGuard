using System;
using Framework;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;

namespace PacketManager.Server.Handlers
{
    public class ClientGatewayShardListPingRequestHandler : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_PING_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);

            return response;
        }
    }
}
