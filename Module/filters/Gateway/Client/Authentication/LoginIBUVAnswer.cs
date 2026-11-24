/*
0x6323 | Gateway | Client -> Server
Name: LoginIBUVAnswer

// Variant: AutoLogin.SendStaticCaptcha
ascii captcha

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x6323)]
    public sealed class LoginIBUVAnswer : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            //string code = packet.ReadAscii();
            return new PacketHandlingResult();
        }
    }
}