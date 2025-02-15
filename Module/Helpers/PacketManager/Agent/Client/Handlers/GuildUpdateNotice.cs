using System;
using System.Text.RegularExpressions;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class GuildUpdateNotice : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            var guildNoticeTitle = packet.ReadAscii();
            var guildNoticeMessage = packet.ReadAscii();

            if (Regex.IsMatch(guildNoticeMessage, @"['""\-]") ||
                Regex.IsMatch(guildNoticeTitle, @"['""\-]"))
            {
                Custom.WriteLine($"Prevented {client.playerInfo.accInfo.username}, attempted SQL injection", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
