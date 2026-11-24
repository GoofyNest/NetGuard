/*
0x70A2 | Agent | Client -> Server
Name: SkillMasteryLearn

// Variant: LearnMasteryHandler.LearnMastery
int masteryId
byte unknown_11 // value: 0x01

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
    [Filter(0x70A2)]
    public sealed class SkillMasteryLearn : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();
            packet.ReadUInt32(); // masteryid
            var level = packet.ReadUInt8();

            if (level > 1)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.Username}, using MasteryLevel exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}