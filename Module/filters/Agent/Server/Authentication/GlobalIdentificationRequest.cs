/*
0x2001 | Agent | Server -> Client
Name: GlobalIdentificationRequest

// Variant: GlobalIdentificationRequest.Invoke
if (!Game.Clientless)
{
    // Stop parsing this variant.
}
ascii serviceName
if (!(serviceName == "GatewayServer"))
{
    if (serviceName == "AgentServer")
    {
        if (selectedAccount == null)
        {
            // Stop parsing this variant.
        }
    }
}

// Variant: SecurityProtocol.Handshake
if (!(m_client_security))
{
    if (!(flags.handshake == 1 && flags.handshake_response == 0))
    {
        ascii m_identity_name
        byte m_identity_flag
    }
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}

// Library/RSBot.Core/GameClientType.cs
enum GameClientType : byte
{
    jSRO = 0,
    Japanese_Old = 1,
    Thailand = 2,
    Vietnam = 3,
    Taiwan_Old = 4,
    Vietnam193 = 5,
    Vietnam274 = 6,
    Chinese_Old = 7,
    Chinese = 8,
    Global = 9,
    Turkey = 10,
    VTC_Game = 11,
    Taiwan = 12,
    Korean = 13,
    Japanese = 14,
    RuSro = 15,
    Rigid = 16,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x2001)]
    public sealed class GlobalIdentificationRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x2001 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
