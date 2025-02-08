using System;
using Framework;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;

namespace PacketManager.Server.Handlers
{
    public class ClientGatewayNoticeRequest : IPacketHandler
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
