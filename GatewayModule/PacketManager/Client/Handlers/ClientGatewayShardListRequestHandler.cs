using System;
using Framework;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;

namespace PacketManager.Server.Handlers
{
    public class ClientGatewayShardListRequestHandler : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            if (packet.GetBytes().Length > 0)
                response.ResultType = PacketResultType.Block;

            Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);

            return response;
        }
    }
}
