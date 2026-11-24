/*
0x9000 | Gateway | Client -> Server
Name: HandshakeAcceptRequest

// Variant: SecurityProtocol.Handshake
if (!(m_client_security))
{
    if (!(flags.handshake == 1 && flags.handshake_response == 0))
    {
    }
}

*/

namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x9000)] // ACCEPT_HANDSHAKE
    public sealed class HandshakeAcceptRequest : IFilter
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