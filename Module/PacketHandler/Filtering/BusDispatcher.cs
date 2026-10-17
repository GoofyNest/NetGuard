using Module.Networking;
using Module.Services;
using System.Collections.Concurrent;

namespace Module.PacketHandler.Filtering
{
    public sealed class BusDispatcher
    {
        private readonly ConcurrentDictionary<string, List<Action<SessionData, BusMessage>>> _handlers = new();

        public void On(string kind, Action<SessionData, BusMessage> handler)
        {
            var list = _handlers.GetOrAdd(kind, _ => new());
            lock (list) list.Add(handler);
        }

        public void Dispatch(SessionData session, BusMessage msg)
        {
            if (!_handlers.TryGetValue(msg.Kind, out var list)) return;
            Action<SessionData, BusMessage>[] snapshot;
            lock (list) snapshot = list.ToArray();
            foreach (var h in snapshot)
            {
                try { h(session, msg); }
                catch (Exception ex) { Custom.WriteLine($"[bus] handler for '{msg.Kind}' threw: {ex.Message}", ConsoleColor.Red); }
            }
        }

        public void Clear() => _handlers.Clear();
    }
}