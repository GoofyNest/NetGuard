using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x34A9)]
    public sealed class MagicOptionGrant : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();
            var _files = Main.Settings.ServerType.CurrentValue;

            if (_files == "jSRO")
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.Username}, using avatar packet on jSRO", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            var avatarBlue = packet.ReadAscii().ToLower();
            if (avatarBlue.Contains("avatar"))
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.Username}, using avatar blue exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}