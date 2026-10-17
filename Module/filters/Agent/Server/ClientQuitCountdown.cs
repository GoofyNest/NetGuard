using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB005)]
    public sealed class ClientQuitCountdown : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            byte unk = packet.ReadUInt8();
            uint unk1 = packet.ReadUInt32();

            byte Action = packet.ReadUInt8();

            client.Agent.LeaveType = Action;

            return response;
        }
    }
}