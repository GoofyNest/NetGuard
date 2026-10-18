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

        private static readonly ILanding?[] _landings = new ILanding?[4];

        public static IFilter? Resolve(FilterContext ctx, ushort opcode)
            => Loader?.Resolve(ctx, opcode);

        public static void SetLanding(FilterContext ctx, ILanding? landing)
        {
            Volatile.Write(ref _landings[(int)ctx], landing);
        }

        public static void ClearLandings()
        {
            for (int i = 0; i < 4; i++) Volatile.Write(ref _landings[i], null);
        }

        /// <summary>
        /// Runs the landing for the given context, if any. Returns null if no
        /// landing is registered OR the landing chose to pass this packet.
        /// </summary>
        public static PacketHandlingResult? RunLanding(
            FilterContext ctx, Packet packet, SessionData client, SessionContext sessionCtx)
        {
            var landing = Volatile.Read(ref _landings[(int)ctx]);
            if (landing == null) return null;
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