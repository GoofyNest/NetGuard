/*
0x70A7 | Agent | Client -> Server
Name: PlayerActivateBerserk

// Variant: Player.EnterBerzerkMode
byte unknown_1073 // value: 0x1

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

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
    [Filter(0x70A7)]
    public sealed class PlayerActivateBerserk : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();
            var flag = packet.ReadUInt8();

            if (flag != 0x01)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.Username}, using Invisible exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}