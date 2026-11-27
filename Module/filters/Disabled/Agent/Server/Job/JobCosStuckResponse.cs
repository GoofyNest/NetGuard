/*
0x30E7 | Agent | Server -> Client
Name: JobCosStuckResponse

// Variant: JobCosStuckResponse.Invoke
byte reason

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/Job/TransportStuckReason.cs
enum TransportStuckReason : byte
{
    TransportDistance = 1,
    CapturedDistance = 2,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30E7)]
    public sealed class JobCosStuckResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x30E7 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
