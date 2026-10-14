namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x5000)]
    public sealed class Handshake : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
            => HandshakeShared.Handle(packet, client);
    }
}