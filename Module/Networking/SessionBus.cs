using Module.PacketHandler.Filtering;
using Module.Services;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Module.Networking
{
    public static class SessionBus
    {
        public static int ApiPort { get; set; }
        public static string ApiPassword { get; set; } = "";

        // Registered sessions in this process (for dispatch)
        private static readonly ConcurrentDictionary<SessionData, byte> _sessions = new();

        // Open connections to peers, keyed by (host, port)
        private static readonly ConcurrentDictionary<(string host, int port), Socket> _peers = new();

        // ---- lifecycle ----

        public static void StartApiListener()
        {
            if (ApiPort <= 0)
                throw new InvalidOperationException("SessionBus.ApiPort is not set.");
            if (string.IsNullOrEmpty(ApiPassword) || ApiPassword.Length < 32)
                throw new InvalidOperationException("SessionBus.ApiPassword is not set (need 32+ hex chars).");

            var listener = new TcpListener(IPAddress.Any, ApiPort);
            listener.Start();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var sock = await listener.AcceptSocketAsync();
                        sock.NoDelay = true;
                        _ = Task.Run(() => HandleIncoming(sock));
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[bus] accept: {ex.Message}", ConsoleColor.Red);
                    }
                }
            });

            Custom.WriteLine($"[bus] listening on 0.0.0.0:{ApiPort}", ConsoleColor.Cyan);
        }

        public static void Register(SessionData s) => _sessions[s] = 0;
        public static void Unregister(SessionData s) => _sessions.TryRemove(s, out _);

        // ---- send ----

        public static void SendTo(string destHost, int destApiPort, string destApiPassword, BusMessage msg)
            => _ = Task.Run(() => SendToAsync(destHost, destApiPort, destApiPassword, msg));

        private static async Task SendToAsync(string destHost, int destApiPort, string destKey, BusMessage msg)
        {
            try
            {
                var kb = Encoding.UTF8.GetBytes(msg.Kind);
                var pb = msg.Payload is null
                    ? []
                    : JsonSerializer.SerializeToUtf8Bytes(msg.Payload);
                var mac = ComputeMac(destKey, kb, pb);

                var buf = new byte[2 + kb.Length + 4 + pb.Length + 2 + mac.Length];
                int o = 0;
                BitConverter.TryWriteBytes(buf.AsSpan(o, 2), (ushort)kb.Length); o += 2; kb.CopyTo(buf, o); o += kb.Length;
                BitConverter.TryWriteBytes(buf.AsSpan(o, 4), (uint)pb.Length); o += 4; pb.CopyTo(buf, o); o += pb.Length;
                BitConverter.TryWriteBytes(buf.AsSpan(o, 2), (ushort)mac.Length); o += 2; mac.CopyTo(buf, o);

                var sock = GetOrCreatePeer(destHost, destApiPort);
                try
                {
                    await sock.SendAsync(buf);
                }
                catch
                {
                    // Connection died — evict and retry once with a fresh socket.
                    _peers.TryRemove((destHost, destApiPort), out _);
                    try { sock.Dispose(); } catch { }

                    sock = GetOrCreatePeer(destHost, destApiPort);
                    await sock.SendAsync(buf);
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[bus] send to {destHost}:{destApiPort}: {ex.Message}", ConsoleColor.Yellow);
            }
        }

        private static Socket GetOrCreatePeer(string host, int port)
        {
            var key = (host, port);

            if (_peers.TryGetValue(key, out var existing))
                return existing;

            // Lock on the key for the actual creation
            lock (_peers)
            {
                if (_peers.TryGetValue(key, out existing))
                    return existing;

                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true
                };
                sock.ConnectAsync(IPAddress.Parse(host), port).GetAwaiter().GetResult();
                _peers[key] = sock;
                return sock;
            }
        }

        // ---- receive ----

        private static async Task HandleIncoming(Socket sock)
        {
            try
            {
                sock.NoDelay = true;
                using var stream = new NetworkStream(sock, ownsSocket: true);
                var buf = new byte[64 * 1024];

                // Loop: read frames until peer disconnects
                while (true)
                {
                    // kindLen
                    if (!await ReadExact(stream, buf, 2)) return;
                    var kindLen = BitConverter.ToUInt16(buf, 0);
                    if (kindLen == 0 || kindLen > 4096) return;   // sanity
                    if (!await ReadExact(stream, buf, kindLen)) return;
                    var kindB = buf.AsSpan(0, kindLen).ToArray();
                    var kind = Encoding.UTF8.GetString(kindB);

                    // payloadLen
                    if (!await ReadExact(stream, buf, 4)) return;
                    var payloadLen = BitConverter.ToInt32(buf, 0);
                    if (payloadLen < 0 || payloadLen > 16 * 1024 * 1024) return;   // sanity
                    var payloadB = payloadLen > 0 ? new byte[payloadLen] : [];
                    if (payloadLen > 0 && !await ReadExact(stream, payloadB, payloadLen)) return;

                    // macLen
                    if (!await ReadExact(stream, buf, 2)) return;
                    var macLen = BitConverter.ToUInt16(buf, 0);
                    if (macLen == 0 || macLen > 128) return;
                    var macB = new byte[macLen];
                    if (!await ReadExact(stream, macB, macLen)) return;

                    // auth
                    var expected = ComputeMac(ApiPassword, kindB, payloadB);
                    if (!CryptographicOperations.FixedTimeEquals(expected, macB))
                    {
                        Custom.WriteLine($"[bus] bad MAC from {sock.RemoteEndPoint} — disconnecting", ConsoleColor.Red);
                        return;
                    }

                    object? obj = payloadB.Length == 0 ? null : JsonSerializer.Deserialize<JsonElement>(payloadB);
                    var msg = new BusMessage(kind, obj);

                    foreach (var session in _sessions.Keys.ToArray())
                    {
                        try { BusRegistry.Dispatcher.Dispatch(session, msg); }
                        catch (Exception ex) { Custom.WriteLine($"[bus] dispatch threw: {ex.Message}", ConsoleColor.Red); }
                    }
                }
            }
            catch { /* peer disconnected / malformed frame */ }
        }

        // ---- helpers ----

        private static byte[] ComputeMac(string hexKey, byte[] kind, byte[] payload)
        {
            using var h = new HMACSHA256(Convert.FromHexString(hexKey));
            h.TransformBlock(kind, 0, kind.Length, null, 0);
            h.TransformFinalBlock(payload, 0, payload.Length);
            return h.Hash!;
        }

        private static async Task<bool> ReadExact(NetworkStream stream, byte[] buf, int n)
        {
            int got = 0;
            while (got < n)
            {
                int r = await stream.ReadAsync(buf.AsMemory(got, n - got));
                if (r <= 0) return false;
                got += r;
            }
            return true;
        }
    }
}