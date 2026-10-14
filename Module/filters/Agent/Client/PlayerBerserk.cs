namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x70A7)]
    public sealed class PlayerBerserk : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            var flag = packet.ReadUInt8();

            if (flag > 1)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, using Invisible exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}