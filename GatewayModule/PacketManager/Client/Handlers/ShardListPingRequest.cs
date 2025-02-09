using System;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.Services;
using SilkroadSecurityAPI;

namespace GatewayModule.PacketManager.Client.Handlers
{
    public class ShardListPingRequest : IPacketHandler
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
