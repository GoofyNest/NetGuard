namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x9000)] // ACCEPT_HANDSHAKE
    public sealed class AcceptHandshake : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult
            {
                SkipSending = false,
                ResultType = PacketResultType.SkipSending
            };
        }
    }
}