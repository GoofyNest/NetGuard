using System;
using Framework;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;

namespace PacketManager.Server.Handlers
{
    public class ClientGatewayLoginRequestHandler : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            byte locale = packet.ReadUInt8();

            Custom.WriteLine($"locale: {locale}", ConsoleColor.DarkMagenta);

            client.StrUserID = packet.ReadAscii();
            client.password = packet.ReadAscii();
            client.serverID = packet.ReadUInt16();

            //if (_client.sent_id != 1 || _client.sent_list != 1)
            //{
            //    Custom.WriteLine($"Sent id: {_client.sent_id} Sent list: {_client.sent_list}", ConsoleColor.Red);
            //    Custom.WriteLine($"Blocked potential exploit from {_client.StrUserID} {_client.password} {_client.ip} for exploiting", ConsoleColor.Yellow);
            //    continue;
            //}

            return response;
        }
    }
}
