/*
0x3053 | Agent | Client -> Server
Name: CharacterResurrectRequest

// Variant: DeadHandler.OnPlayerDied
if (Game.Player.Level < 10)
{
    byte unknown_41 // value: 2
}

// Variant: DeadHandler.OnPlayerDied
byte unknown_73 // value: 1

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/LifeState.cs
enum LifeState : int
{
    Alive = 1,
    Dead = 2,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x3053)]
    public sealed class CharacterResurrectRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
