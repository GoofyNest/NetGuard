using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering
{
    internal sealed class DynamicFilterAdapter(IFilter inner) : IPacketHandler
    {
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<IFilter, DynamicFilterAdapter> Cache = new();
        internal static DynamicFilterAdapter For(IFilter filter) => Cache.GetValue(filter, static f => new(f));
        private readonly IFilter _inner = inner;

        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
            => _inner.Handle(packet, client, ctx);
    }

    internal sealed class StaticResultHandler(PacketHandlingResult r) : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx) => r;
    }
}