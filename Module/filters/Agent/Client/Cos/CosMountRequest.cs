/*
0x70CB | Agent | Client -> Server
Name: CosMountRequest

// Variant: Cos.Mount
byte unknown_143 // value: 1
int uniqueId

// Variant: Cos.Dismount
byte unknown_153 // value: 0
int uniqueId

// Variant: Transport.Dismount
byte unknown_13 // value: 0
uint uniqueId

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

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x70CB)]
    public sealed class CosMountRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
