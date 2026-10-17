using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB006)]
    public sealed class ClientCancelQuit : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            client.Agent.LeaveType = 0x0;

            return response;
        }
    }
}