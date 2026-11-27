/*
0xB04B | Agent | Server -> Client
Name: ActionDeselectResponse

// Variant: ActionDeselectResponse.Invoke
byte unknown_30
if (unknown_30 != 1)
{
    // Stop parsing this variant.
}

// Response fields checked by SpawnedBionic.TryDeselect
byte successFlag
if (successFlag == 2)
{
    ushort errorCode
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

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB04B)]
    public sealed class ActionDeselectResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
