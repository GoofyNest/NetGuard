using System;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Client;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Server;

namespace Module.Helpers.PacketManager.Gateway.Client.Handlers
{
    public class ShardListRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            if (packet.GetBytes().Length > 0)
                response.ResultType = PacketResultType.Block;

            Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);

            return response;
        }
    }
}
