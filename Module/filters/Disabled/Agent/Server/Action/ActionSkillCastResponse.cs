/*
0xB070 | Agent | Server -> Client
Name: ActionSkillCastResponse

// Variant: ActionSkillCastResponse.Invoke
byte result
if (result != 0x01)
{
    byte errorCode
    // Stop parsing this variant.
}
byte actionCode
if (gameVersion > Thailand)
{
    byte unknown_70 // always 0x30
}
uint skillId
uint executorId
uint id
if (gameVersion > Chinese && gameVersion != Japanese)
{
    uint unknownId
}
uint targetId
if (gameVersion == Turkey || gameVersion == Global || gameVersion == VTC_Game || gameVersion == RuSro || gameVersion == Korean || gameVersion == Japanese || gameVersion == Taiwan)
{
    byte unknown_94
    byte flag
}
if (!(gameVersion == Turkey || gameVersion == Global || gameVersion == VTC_Game || gameVersion == RuSro || gameVersion == Korean || gameVersion == Japanese || gameVersion == Taiwan))
{
    if (gameVersion == Rigid)
    {
        byte flag
        byte flag
    }
    if (!(gameVersion == Rigid))
    {
        byte flag
    }
}
if (Flag.HasFlag(ActionStateFlag.Attack))
{
    byte hitCount
    byte affectedObjectCount
    repeat (var i = 0; i < affectedObjectCount; i++)
    {
        uint uniqueId
        repeat (var j = 0; j < hitCount; j++)
        {
            byte state
            if (state != ActionHitStateFlag.Block)
            {
                byte critStatus // 0x01: normal 0x02 critical
                byte damage
                byte damage
                byte damage
                int unknownState
            }
            if (state.HasFlag(ActionHitStateFlag.KnockBack))
            {
                ushort region
                int xOffset
                int zOffset
                int yOffset
            }
        }
    }
}
if (Flag.HasFlag(ActionStateFlag.Teleport))
{
    ushort region
    int xOffset
    int zOffset
    int yOffset
    if (!(PlayerIsExecutor))
    {
        if (!TryGetExecutor<SpawnedBionic>(out var executor))
        {
            // Stop parsing this variant.
        }
    }
}
if (action.PlayerIsExecutor)
{
    // Stop parsing this variant.
}
if (!action.TryGetExecutor<SpawnedBionic>(out var executor))
{
    // Stop parsing this variant.
}
if (!action.PlayerIsTarget)
{
    // Stop parsing this variant.
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/ActionHitStateFlag.cs
enum ActionHitStateFlag : byte
{
    Damage = 1,
    Block = 2,
    KnockDown = 4,
    KnockBack = 5,
    Abort = 8,
    Dead = 128,
    KnockDownDead = 132,
    KnockBackDead = 133,
}

// Library/RSBot.Core/Objects/ActionStateFlag.cs
[Flags]
enum ActionStateFlag : byte
{
    None = 0,
    Attack = 1,
    Teleport = 8,
}

// Library/RSBot.Core/Objects/LifeState.cs
enum LifeState : int
{
    Alive = 1,
    Dead = 2,
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
    [Filter(0xB070)]
    public sealed class ActionSkillCastResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
