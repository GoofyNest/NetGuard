/*
0xB0A2 | Agent | Server -> Client
Name: SkillMasteryLearnResponse

// Variant: SkillMasteryLearnResponse.Invoke
byte result
if (result != 1)
{
    // Stop parsing this variant.
}
uint masteryId
byte level

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB0A2)]
    public sealed class SkillMasteryLearnResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB0A2 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
