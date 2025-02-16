using System;
using System.Text.RegularExpressions;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public partial class GuildUpdateNotice : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var guildNoticeTitle = packet.ReadAscii();
            var guildNoticeMessage = packet.ReadAscii();

            if (SQLInjectPRevention().IsMatch(guildNoticeMessage) ||
                SQLInjectPRevention().IsMatch(guildNoticeTitle))
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, attempted SQL injection", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }

        [GeneratedRegex(@"['""\-]")]
        private partial Regex SQLInjectPRevention();
    }
}
