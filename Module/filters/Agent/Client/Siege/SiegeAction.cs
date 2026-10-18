using System.Text.RegularExpressions;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x705E)]
    public sealed class FortressSiegeAction : IFilter
    {
        private static readonly Regex SQLInjectPrevention = new(@"['""\-]", RegexOptions.Compiled);

        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();
            var message = packet.ReadAscii();

            if (SQLInjectPrevention.IsMatch(message))
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.Username}, attempted SQL injection", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}