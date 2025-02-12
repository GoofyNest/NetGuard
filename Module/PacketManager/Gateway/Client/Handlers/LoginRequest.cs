using System;
using SilkroadSecurityAPI;
using Module.Framework;
using Module.Engine.Classes;
using Module.Services;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Client.Handlers
{
    public class LoginRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            byte locale = packet.ReadUInt8();

            Custom.WriteLine($"locale: {locale}", ConsoleColor.DarkMagenta);

            client.StrUserID = packet.ReadAscii();
            client.password = packet.ReadAscii();
            client.serverID = packet.ReadUInt16();

            if (client.sent_id != 1 || client.sent_list != 1)
            {
                Custom.WriteLine($"Sent id: {client.sent_id}", ConsoleColor.DarkMagenta);
                Custom.WriteLine($"Sent list: {client.sent_list}", ConsoleColor.DarkMagenta);

                Custom.WriteLine($"Blocked potential exploit from {client.StrUserID} {client.password} {client.ip} for exploiting", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
