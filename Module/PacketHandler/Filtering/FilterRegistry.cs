using Module.Networking;
using Module.Services;
using SilkroadSecurityAPI;
using System.Collections.Concurrent;

namespace Module.PacketHandler.Filtering
{
    public static class FilterRegistry
    {
        public static FilterLoader? Loader { get; set; }

        public static readonly ConcurrentDictionary<int, SessionData> LiveSessions = new();

        private static readonly ConcurrentDictionary<FilterContext, ILanding> _landings = new();

        public static IFilter? Resolve(FilterContext ctx, ushort opcode)
            => Loader?.Resolve(ctx, opcode);

        public static void SetLanding(FilterContext ctx, ILanding? landing)
        {
            if (landing == null) _landings.TryRemove(ctx, out _);
            else _landings[ctx] = landing;
        }

        public static void ClearLandings() => _landings.Clear();

        /// <summary>
        /// Runs the landing for the given context, if any. Returns null if no
        /// landing is registered OR the landing chose to pass this packet.
        /// </summary>
        public static PacketHandlingResult? RunLanding(
            FilterContext ctx, Packet packet, SessionData client, SessionContext sessionCtx)
        {
            if (!_landings.TryGetValue(ctx, out var landing)) return null;
            try { return landing.Handle(packet, client, sessionCtx); }
            catch (Exception ex)
            {
                Custom.WriteLine(
                    $"[landing] {ctx} threw on 0x{packet.Opcode:X4}: {ex.Message}",
                    ConsoleColor.Red);
                return null;
            }
        }
    }
}