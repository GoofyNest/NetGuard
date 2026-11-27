/*
0xB0CB | Agent | Server -> Client
Name: UpdateMountStateResponse

// Variant: UpdateMountStateResponse.Invoke
byte unknown_13
if (unknown_13 != 0x01)
{
    // Stop parsing this variant.
}
uint ownerUniqueId
byte isMounted // boolean: 0=false, nonzero=true
uint cosUniqueId
if (ownerUniqueId == Game.Player.UniqueId)
{
    if (!isMounted)
    {
        // Stop parsing this variant.
    }
}
if (!SpawnManager.TryGetEntity<SpawnedPlayer>(ownerUniqueId, out var owner))
{
    // Stop parsing this variant.
}
if (!SpawnManager.TryGetEntity<SpawnedCos>(cosUniqueId, out var cos))
{
    // Stop parsing this variant.
}

// Response fields checked by Cos.Dismount
byte result

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
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
    [Filter(0xB0CB)]
    public sealed class UpdateMountStateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
