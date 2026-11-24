/*
0xB025 | Agent | Server -> Client
Name: ChatResponseHook

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB025)]
    public sealed class ChatResponseHook : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB025 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
