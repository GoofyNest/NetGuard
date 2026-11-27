/*
0xB0BD | Agent | Server -> Client
Name: ActionBuffAddResponse

// Variant: ActionBuffAddResponse.Invoke
uint targetId
uint skillId
uint token
if (token == 0)
{
    // Stop parsing this variant.
}
if (targetId == Game.Player.UniqueId)
{
    // Stop parsing this variant.
}

// Response fields checked by SkillManager.CastBuff
uint targetId
uint castedSkillId

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/ActionTarget.cs
enum ActionTarget : int
{
    None = 0,
    Entity = 1,
    Area = 2,
}

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
    [Filter(0xB0BD)]
    public sealed class ActionBuffAddResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
