/*
0x30BF | Agent | Server -> Client
Name: EntityUpdateStateResponse

// Variant: EntityUpdateStateResponse.Invoke
uint uniqueId
byte type
byte state
if (!SpawnManager.TryGetEntityIncludingMe(uniqueId, out var entity))
{
    // Stop parsing this variant.
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/BattleState.cs
enum BattleState : int
{
    InPeace = 0,
    InBattle = 1,
}

// Library/RSBot.Core/Objects/BodyState.cs
enum BodyState : int
{
    None = 0,
    Hwan = 1,
    Untouchable = 2,
    GameMasterInvincible = 3,
    GameMasterInvisible = 4,
    Berzerk = 5,
    Stealth = 6,
    Invisible = 7,
}

// Library/RSBot.Core/Objects/LifeState.cs
enum LifeState : int
{
    Alive = 1,
    Dead = 2,
}

// Library/RSBot.Core/Objects/MotionState.cs
enum MotionState : int
{
    None = 0,
    Walking = 2,
    Running = 3,
    Sitting = 4,
}

// Library/RSBot.Core/Objects/MovementType.cs
enum MovementType : int
{
    Walking = 0,
    Running = 1,
}

// Library/RSBot.Core/Objects/PvpState.cs
enum PvpState : int
{
    None = 0,
    Purple = 1,
    Red = 2,
    Unknown = 3,
}

// Library/RSBot.Core/Objects/ScrollState.cs
enum ScrollState : int
{
    Cancel = 0,
    NormalScroll = 1,
    ThiefScroll = 2,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30BF)]
    public sealed class EntityUpdateStateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x30BF handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
