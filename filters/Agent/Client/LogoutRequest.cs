namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7005)]
    public sealed class LogoutRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            if (client.Agent.InCharSelectionScreen)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, attempt to crash people", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            byte action = packet.ReadUInt8();

            if (action > 2)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, attempt to crash people", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            switch (action)
            {
                case 0x01: break;
                case 0x02: break;
                default:
                    response.ResultType = PacketResultType.Block;
                    break;
            }

            return response;
        }
    }
}