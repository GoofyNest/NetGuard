/*
0xB074 | Agent | Server -> Client
Name: ActionCommandStateResponse

// Variant: ActionCommandStateResponse.Invoke
byte state
byte recurring

// Response fields checked by SkillManager.CastSkillOld
byte predicate
byte predicate

// Response fields checked by SkillManager.CastBuff
byte unknown_459
byte unknown_459

// Response fields checked by SkillManager.CastSkillAt
byte unknown_519
byte unknown_519

// Response fields checked by SkillManager.CancelAction
byte unknown_620
byte unknown_620

// Response fields checked by SpawnedItem.Pickup
byte actionState
byte repeatAction // boolean: 0=false, nonzero=true

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/ActionCommandType.cs
enum ActionCommandType : byte
{
    Execute = 1,
    Cancel = 2,
}

// Library/RSBot.Core/Objects/ActionState.cs
enum ActionState : byte
{
    Begin = 1,
    End = 2,
    Error = 3,
}

// Library/RSBot.Core/Objects/ActionTarget.cs
enum ActionTarget : int
{
    None = 0,
    Entity = 1,
    Area = 2,
}

// Library/RSBot.Core/Objects/ActionType.cs
enum ActionType : byte
{
    Attack = 1,
    Pickup = 2,
    Trace = 3,
    Cast = 4,
    Dispel = 5,
}

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
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB074)]
    public sealed class ActionCommandStateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB074 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
