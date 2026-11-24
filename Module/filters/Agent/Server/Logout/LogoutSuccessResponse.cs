/*
0x300A | Agent | Server -> Client
Name: LogoutSuccessResponse

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x300A)]
    public sealed class LogoutSuccessResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x300A handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
