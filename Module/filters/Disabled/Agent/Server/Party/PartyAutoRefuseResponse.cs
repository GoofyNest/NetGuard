/*
0xB067 | Agent | Server -> Client
Name: PartyAutoRefuseResponse

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB067)]
    public sealed class PartyAutoRefuseResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB067 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
