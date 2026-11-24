/*
0xB021 | Agent | Server -> Client
Name: EntityUpdateMovementResponse

// Variant: EntityUpdateMovementResponse.Invoke
uint uniqueId
byte hasDestination // boolean: 0=false, nonzero=true
if (result.HasDestination)
{
    ushort region
    if (!position.Region.IsDungeon)
    {
        short xOffset
        short zOffset
        short yOffset
    }
    if (!(!position.Region.IsDungeon))
    {
        int xOffset
        int zOffset
        int yOffset
    }
}
if (!(result.HasDestination))
{
    byte unknown_67 // 0 = Spinning, 1 = Sky-/Key-walking
    short angle
}
byte hasSource // boolean: 0=false, nonzero=true
if (result.HasSource)
{
    ushort region
    if (result.Source.Region.IsDungeon)
    {
        int xOffset
        float zOffset
        int yOffset
    }
    if (!(result.Source.Region.IsDungeon))
    {
        short xOffset
        float zOffset
        short yOffset
    }
}
if (uniqueId == Game.Player.UniqueId)
{
    if (movement.HasAngle)
    {
        // Stop parsing this variant.
    }
    // Stop parsing this variant.
}
if (!SpawnManager.TryGetEntity<SpawnedEntity>(uniqueId, out var entity))
{
    // Stop parsing this variant.
}
if (movement.HasAngle)
{
    // Stop parsing this variant.
}

// Response fields checked by Player.MoveTo
uint uniqueId

// Response fields checked by Cos.MoveTo
uint predicate

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
}

// Library/RSBot.Core/Objects/Cos/CosCommand.cs
enum CosCommand : byte
{
    Move = 1,
    Attack = 2,
    Pickup = 8,
    Follow = 9,
    Charm = 11,
    Cast = 13,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB021)]
    public sealed class EntityUpdateMovementResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB021 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
