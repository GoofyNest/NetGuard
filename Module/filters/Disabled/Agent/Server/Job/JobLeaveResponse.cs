/*
0xB0E2 | Agent | Server -> Client
Name: JobLeaveResponse

// Variant: JobLeaveResponse.Invoke
byte result
if (result != 1)
{
    // Stop parsing this variant.
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/JobType.cs
enum JobType : int
{
    None = 0,
    Trade = 1,
    Thief = 2,
    Hunter = 3,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB0E2)]
    public sealed class JobLeaveResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB0E2 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
