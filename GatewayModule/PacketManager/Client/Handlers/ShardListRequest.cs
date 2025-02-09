using System;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.Services;
using SilkroadSecurityAPI;

namespace GatewayModule.PacketManager.Client.Handlers
{
    public class ShardListRequest : IPacketHandler
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
