/*
0x5000 | Agent | Client -> Server
Name: HandshakeRequest

// Variant: SecurityProtocol.GenerateSecurity
byte m_security_flag
if (m_security_flags.blowfish == 1)
{
    ulong m_initial_blowfish_key
}
if (m_security_flags.security_bytes == 1)
{
    uint m_seed_count
    uint m_crc_seed
}
if (m_security_flags.handshake == 1)
{
    ulong m_handshake_blowfish_key
    uint m_value_g
    uint m_value_p
    uint m_value_A
}

// Variant: SecurityProtocol.Handshake
if (m_client_security)
{
    byte tmp_flag
    ulong m_challenge_key
}

// Variant: SecurityProtocol.Handshake
if (!(m_client_security))
{
    if (flags.handshake == 1 && flags.handshake_response == 0)
    {
        uint m_value_B
        ulong m_client_key
    }
}

*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x5000)]
    public sealed class HandshakeRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
            => HandshakeShared.Handle(packet, client);
    }
}