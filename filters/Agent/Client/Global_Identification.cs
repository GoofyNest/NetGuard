namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x2001)]
    public sealed class Global_Identification : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            if (packet.GetBytes().Length != 12)
            {
                Custom.WriteLine($"Ignore packet GLOBAL_IDENTIFICATION from {client.PlayerInfo.IpInfo.Ip}", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }
            else
            {
                Custom.WriteLine($"Should recieve?");
                response.ResultType = PacketResultType.DoReceive;
            }

            return response;
        }
    }
}