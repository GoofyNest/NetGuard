namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x6102)]
    public sealed class LoginRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            byte locale = packet.ReadUInt8();
            Custom.WriteLine($"locale: {locale}", ConsoleColor.Magenta);

            client.PlayerInfo.Username = packet.ReadAscii();
            client.PlayerInfo.Password = packet.ReadAscii();
            client.PlayerInfo.ServerID = packet.ReadUInt16();
            client.PlayerInfo.Timestamp = DateTime.UtcNow;

            if (!client.Gateway.SentPatchResponse)
            {
                Custom.WriteLine($"sentPatchResponse: {client.Gateway.SentPatchResponse}", ConsoleColor.Magenta);
                Custom.WriteLine($"sentShardListResponse: {client.Gateway.SentShardListResponse}", ConsoleColor.Magenta);
                Custom.WriteLine($"Blocked potential exploit from {client.PlayerInfo.Username} {client.PlayerInfo.Password} {client.PlayerInfo.IpInfo.Ip} for exploiting", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}