using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering
{
    internal sealed class DynamicFilterAdapter : IPacketHandler
    {
        private readonly IFilter _inner;
        public DynamicFilterAdapter(IFilter inner) => _inner = inner;
        public PacketHandlingResult Handle(Packet packet, SessionData client)
            => _inner.Handle(packet, client);
    }
}