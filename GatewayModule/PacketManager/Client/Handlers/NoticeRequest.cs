using System;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.Services;
using GatewayModule.SilkroadSecurityAPI;

namespace GatewayModule.PacketManager.Client.Handlers
{
    public class NoticeRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            byte contentID = packet.ReadUInt8();

            if (packet.GetBytes().Length > 1)
                response.ResultType = PacketResultType.Block;

            Custom.WriteLine($"CLIENT_GATEWAY_NOTICE_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);

            return response;
        }
    }
}
