namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x5000)] // LOGIN_SERVER_HANDSHAKE
    public sealed class Handshake : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            return new PacketHandlingResult
            {
                SkipSending = false,
                ResultType = PacketResultType.SkipSending
            };
        }
    }
}