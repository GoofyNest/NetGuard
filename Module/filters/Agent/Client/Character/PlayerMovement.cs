/*
0x7021 | Agent | Client -> Server
Name: PlayerMovement

// Variant: Player.MoveTo
byte unknown_696 // value: 1
if (!Game.Player.IsInDungeon)
{
    ushort id
    short xOffset
    short zOffset
    short yOffset
}
if (!(!Game.Player.IsInDungeon))
{
    ushort id
    int xOffset
    int zOffset
    int yOffset
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

using NetGuard.Hot;
namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7021)]
    public sealed class PlayerMovement : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();
            
            return response;
        }
    }
}