/*
0x7001 | Agent | Client -> Server
Name: CharacterSelectionJoinRequest

// Variant: CharacterSelectionJoinRequest.Invoke
ascii characterName

// Variant: AutoLogin.EnterGame
ascii character

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7001)]
    public sealed class CharacterSelectionJoinRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
