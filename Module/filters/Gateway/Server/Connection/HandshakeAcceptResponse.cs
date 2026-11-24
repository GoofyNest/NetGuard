/*
0x9000 | Gateway | Server -> Client
Name: HandshakeAcceptResponse

// Variant: SecurityProtocol.Handshake
if (!(m_client_security))
{
    if (!(flags.handshake == 1 && flags.handshake_response == 0))
    {
    }
}

*/

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0x9000)]
    public sealed class HandshakeAcceptResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x9000 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
