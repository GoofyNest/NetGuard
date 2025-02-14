using System;
using System.Text.RegularExpressions;
using Module.Engine.Classes;
using Module.Framework;
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
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
