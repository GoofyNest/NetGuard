/*
0xB516 | Agent | Server -> Client
Name: EntityUpdatePvpFlag

// Variant: EntityUpdatePvpFlag.Invoke
byte unknown_15 // boolean: 0=false, nonzero=true
if (!unknown_15)
{
    // Stop parsing this variant.
}
uint uniqueId
byte flag
if (Game.Player.UniqueId == uniqueId)
{
    // Stop parsing this variant.
}
if (entity == null)
{
    // Stop parsing this variant.
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/PvpFlag.cs
enum PvpFlag : int
{
    None = 0,
    Red = 1,
    Gray = 2,
    Blue = 3,
    White = 4,
    Gold = 5,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB516)]
    public sealed class EntityUpdatePvpFlag : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
