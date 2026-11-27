/*
0xB045 | Agent | Server -> Client
Name: ActionSelectResponse

// Variant: ActionSelectResponse.Invoke
byte unknown_31
if (unknown_31 != 0x01)
{
    // Stop parsing this variant.
}
uint uniqueId
if (!SpawnManager.TryGetEntity<SpawnedBionic>(uniqueId, out var entity))
{
    // Stop parsing this variant.
}
if (entity is SpawnedMonster)
{
    byte hasHealth // boolean: 0=false, nonzero=true
    if (hasHealth)
    {
        int health
    }
}
if (!(entity is SpawnedMonster))
{
    if (entity is SpawnedNpcNpc)
    {
        byte hasHealth // boolean: 0=false, nonzero=true
        if (hasHealth)
        {
            int health
        }
    }
}

// Response fields checked by SpawnedBionic.TrySelect
byte result
if (result)
{
    uint unknown_133
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
}

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
    [Filter(0xB045)]
    public sealed class ActionSelectResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
