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

            byte locale = packet.ReadUInt8();

            Custom.WriteLine($"locale: {locale}", ConsoleColor.DarkMagenta);

            client.PlayerInfo.AccInfo = new();

            var accInfo = client.PlayerInfo.AccInfo;

            accInfo.Username = packet.ReadAscii();
            accInfo.Password = packet.ReadAscii();

            client.Gateway.ServerID = packet.ReadUInt16();

            if (!client.Gateway.SentPatchResponse || !client.Gateway.SentShardListResponse)
            {
                Custom.WriteLine($"sentPatchResponse: {client.Gateway.SentPatchResponse}", ConsoleColor.DarkMagenta);
                Custom.WriteLine($"sentShardListResponse: {client.Gateway.SentShardListResponse}", ConsoleColor.DarkMagenta);

                Custom.WriteLine($"Blocked potential exploit from {accInfo.Username} {accInfo.Password} {client.PlayerInfo.IpInfo.Ip} for exploiting", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
