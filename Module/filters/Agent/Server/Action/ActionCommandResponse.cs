/*
0xB071 | Agent | Server -> Client
Name: ActionCommandResponse

// Variant: ActionCommandResponse.Invoke
byte unknown_30
if (unknown_30 != 0x01)
{
    // Stop parsing this variant.
}
uint id // ActionId
uint targetId // originalTargetId
byte flag
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
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB071)]
    public sealed class ActionCommandResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB071 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
