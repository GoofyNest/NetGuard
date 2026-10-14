namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x9000)]
    public sealed class AcceptHandshake : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
            => HandshakeShared.Handle(packet, client);
    }
}