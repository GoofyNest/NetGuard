using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x70A7)]
    public sealed class PlayerActivateBerserk : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();
            var flag = packet.ReadUInt8();

            if (flag != 0x01)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.Username}, using Invisible exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}