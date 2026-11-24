/*
0x2001 | Gateway | Client -> Server
Name: GlobalIdentificationRequest

// Variant: SecurityProtocol.Handshake
if (!(m_client_security))
{
    if (!(flags.handshake == 1 && flags.handshake_response == 0))
    {
        ascii m_identity_name
        byte m_identity_flag
    }
}

*/

namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x2001)]
    public sealed class GlobalIdentificationRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            if (packet.GetBytes().Length != 12)
            {
                Custom.WriteLine($"Ignore packet GLOBAL_IDENTIFICATION from {client.PlayerInfo.IpInfo.Ip}", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }
            else
            {
                Custom.WriteLine($"Should recieve?");
                response.ResultType = PacketResultType.DoReceive;
            }

            return response;
        }
    }
}