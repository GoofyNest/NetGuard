namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x6103)]
    public sealed class AuthRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            packet.ReadUInt32(); // token
            client.PlayerInfo.Username = packet.ReadAscii();
            client.PlayerInfo.Password = packet.ReadAscii();
            packet.ReadUInt8(); // operation type

            var mac = packet.ReadUInt8Array(6);
            client.PlayerInfo.Mac = BitConverter.ToString(mac);

            return response;
        }
    }
}