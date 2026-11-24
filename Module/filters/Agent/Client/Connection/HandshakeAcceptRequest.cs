/*
0x9000 | Agent | Client -> Server
Name: HandshakeAcceptRequest

// Variant: SecurityProtocol.Handshake
if (!(m_client_security))
{
    if (!(flags.handshake == 1 && flags.handshake_response == 0))
    {
    }
}

*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x9000)]
    public sealed class HandshakeAcceptRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
            => HandshakeShared.Handle(packet, client);
    }
}