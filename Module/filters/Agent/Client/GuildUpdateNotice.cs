using System.Text.RegularExpressions;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x70F9)]
    public sealed class GuildUpdateNotice : IFilter
    {
        private static readonly Regex SQLInjectPrevention = new(@"['""\-]", RegexOptions.Compiled);

        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            var guildNoticeTitle = packet.ReadAscii();
            var guildNoticeMessage = packet.ReadAscii();

            if (SQLInjectPrevention.IsMatch(guildNoticeMessage) ||
                SQLInjectPrevention.IsMatch(guildNoticeTitle))
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.Username}, attempted SQL injection", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}