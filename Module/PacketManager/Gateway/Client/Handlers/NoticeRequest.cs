using System;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Client.Handlers
{
    public class NoticeRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            byte contentID = packet.ReadUInt8();

            if (packet.GetBytes().Length > 1)
                response.ResultType = PacketResultType.Block;

            Custom.WriteLine($"CLIENT_GATEWAY_NOTICE_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);

            return response;
        }
    }
}
