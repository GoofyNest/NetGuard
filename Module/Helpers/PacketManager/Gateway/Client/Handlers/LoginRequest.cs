using System;
using SilkroadSecurityAPI;
using Module.Framework;
using Module.Engine.Classes;
using Module.Services;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Client;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Server;

namespace Module.Helpers.PacketManager.Gateway.Client.Handlers
{
    public class LoginRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            byte locale = packet.ReadUInt8();

            Custom.WriteLine($"locale: {locale}", ConsoleColor.DarkMagenta);

            client.playerInfo.accInfo = new();

            var accInfo = client.playerInfo.accInfo;

            accInfo.username = packet.ReadAscii();
            accInfo.password = packet.ReadAscii();

            client.gatewaySettings.serverID = packet.ReadUInt16();

            if (!client.gatewaySettings.sentPatchResponse || !client.gatewaySettings.sentShardListResponse)
            {
                Custom.WriteLine($"sentPatchResponse: {client.gatewaySettings.sentPatchResponse}", ConsoleColor.DarkMagenta);
                Custom.WriteLine($"sentShardListResponse: {client.gatewaySettings.sentShardListResponse}", ConsoleColor.DarkMagenta);

                Custom.WriteLine($"Blocked potential exploit from {accInfo.username} {accInfo.password} {client.playerInfo.ipInfo.ip} for exploiting", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
