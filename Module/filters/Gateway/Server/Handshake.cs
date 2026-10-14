namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0x5000)] // LOGIN_SERVER_HANDSHAKE
    public sealed class Handshake : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            return new PacketHandlingResult
            {
                SkipSending = true,
                ResultType = PacketResultType.SkipSending
            };
        }
    }
}