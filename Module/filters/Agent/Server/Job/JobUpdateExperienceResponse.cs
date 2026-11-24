/*
0x30E6 | Agent | Server -> Client
Name: JobUpdateExperienceResponse

// Variant: JobUpdateExperienceResponse.Invoke
byte type
byte level
uint experience

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
    [Filter(0x30E6)]
    public sealed class JobUpdateExperienceResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x30E6 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
