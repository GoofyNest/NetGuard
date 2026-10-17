using Module.Networking;
using System.Collections.Concurrent;

namespace Module.PacketHandler.Filtering
{
    public static class FilterRegistry
    {
        public static FilterLoader? Loader { get; set; }

        public static readonly ConcurrentDictionary<int, SessionData> LiveSessions = new();

        public static IFilter? Resolve(FilterContext ctx, ushort opcode)
            => Loader?.Resolve(ctx, opcode);
    }
}