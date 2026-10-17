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
        // Set once at startup by Main.StartProgram:
        public static int ApiPort { get; set; }
        public static string ApiPassword { get; set; } = "";   // MY key (hex)

        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<SessionData, byte>> _byIp = new();
        private static readonly ConcurrentDictionary<SessionData, ConcurrentQueue<BusMessage>> _queues = new();

        // ---- lifecycle ----

        public static void StartApiListener()
        {
            if (ApiPort <= 0)
                throw new InvalidOperationException("SessionBus.ApiPort is not set.");
            if (string.IsNullOrEmpty(ApiPassword) || ApiPassword.Length < 32)
                throw new InvalidOperationException("SessionBus.ApiPassword is not set (need 32+ hex chars).");

            var listener = new TcpListener(IPAddress.Loopback, ApiPort);
            listener.Start();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var sock = await listener.AcceptSocketAsync();
                        _ = Task.Run(() => HandleIncoming(sock));
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[bus] accept: {ex.Message}", ConsoleColor.Red);
                    }
                }
            });

            Custom.WriteLine($"[bus] listening on 127.0.0.1:{ApiPort}", ConsoleColor.Cyan);
        }

        public static void Register(SessionData s)
        {
            var ip = s.PlayerInfo.IpInfo.Ip;
            if (string.IsNullOrEmpty(ip)) return;
            _byIp.GetOrAdd(ip, _ => new())[s] = 0;
            _queues[s] = new();
        }

        public static void Unregister(SessionData s)
        {
            var ip = s.PlayerInfo.IpInfo.Ip;
            if (!string.IsNullOrEmpty(ip) && _byIp.TryGetValue(ip, out var set))
            {
                set.TryRemove(s, out _);
                if (set.IsEmpty) _byIp.TryRemove(ip, out _);
            }
            _queues.TryRemove(s, out _);
        }

        // ---- send ----

        /// <summary>
        /// Send to another module.
        /// destApiPort: the target module's ModuleSettings.ApiPort.
        /// destApiPassword: the target module's ModuleSettings.ApiPassword.
        /// </summary>
        // SessionBus
        public static void SendTo(SessionData session, int destApiPort, string destApiPassword, BusMessage msg)
            => _ = Task.Run(() => SendToAsync(session, destApiPort, destApiPassword, msg));

        private static async Task SendToAsync(SessionData session, int destApiPort, string destKey, BusMessage msg)
        {
            try
            {
                var kb = Encoding.UTF8.GetBytes(msg.Kind);
                var ib = Encoding.UTF8.GetBytes(session.PlayerInfo.IpInfo.Ip);   // <-- from session, not static
                var pb = msg.Payload is null
                    ? Array.Empty<byte>()
                    : JsonSerializer.SerializeToUtf8Bytes(msg.Payload);
                var mac = ComputeMac(destKey, kb, ib, pb);

                using var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await sock.ConnectAsync(IPAddress.Loopback, destApiPort);
                using var stream = new NetworkStream(sock, ownsSocket: false);

                var buf = new byte[2 + kb.Length + 2 + ib.Length + 4 + pb.Length + 2 + mac.Length];
                int o = 0;
                BitConverter.TryWriteBytes(buf.AsSpan(o, 2), (ushort)kb.Length); o += 2; kb.CopyTo(buf, o); o += kb.Length;
                BitConverter.TryWriteBytes(buf.AsSpan(o, 2), (ushort)ib.Length); o += 2; ib.CopyTo(buf, o); o += ib.Length;
                BitConverter.TryWriteBytes(buf.AsSpan(o, 4), (uint)pb.Length); o += 4; pb.CopyTo(buf, o); o += pb.Length;
                BitConverter.TryWriteBytes(buf.AsSpan(o, 2), (ushort)mac.Length); o += 2; mac.CopyTo(buf, o);

                await stream.WriteAsync(buf);
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[bus] send to :{destApiPort}: {ex.Message}", ConsoleColor.Yellow);
            }
        }

        // ---- receive ----

        private static async Task HandleIncoming(Socket sock)
        {
            try
            {
                using var stream = new NetworkStream(sock, ownsSocket: true);
                var buf = new byte[64 * 1024];

                if (!await ReadExact(stream, buf, 2)) return;
                var kindLen = BitConverter.ToUInt16(buf, 0);
                if (!await ReadExact(stream, buf, kindLen)) return;
                var kindB = buf.AsSpan(0, kindLen).ToArray();
                var kind = Encoding.UTF8.GetString(kindB);

                if (!await ReadExact(stream, buf, 2)) return;
                var ipLen = BitConverter.ToUInt16(buf, 0);
                if (!await ReadExact(stream, buf, ipLen)) return;
                var ipB = buf.AsSpan(0, ipLen).ToArray();
                var ip = Encoding.UTF8.GetString(ipB);

                if (!await ReadExact(stream, buf, 4)) return;
                var payloadLen = BitConverter.ToInt32(buf, 0);
                var payloadB = payloadLen > 0 ? new byte[payloadLen] : Array.Empty<byte>();
                if (payloadLen > 0 && !await ReadExact(stream, payloadB, payloadLen)) return;

                if (!await ReadExact(stream, buf, 2)) return;
                var macLen = BitConverter.ToUInt16(buf, 0);
                var macB = new byte[macLen];
                if (!await ReadExact(stream, macB, macLen)) return;

                var expected = ComputeMac(ApiPassword, kindB, ipB, payloadB);   // MY key
                if (!CryptographicOperations.FixedTimeEquals(expected, macB))
                {
                    Custom.WriteLine($"[bus] rejected: bad MAC from {sock.RemoteEndPoint}", ConsoleColor.Red);
                    return;
                }

                object? obj = payloadB.Length == 0 ? null : JsonSerializer.Deserialize<JsonElement>(payloadB);
                var msg = new BusMessage(kind, obj);

                if (_byIp.TryGetValue(ip, out var set))
                {
                    foreach (var s in set.Keys)
                        if (_queues.TryGetValue(s, out var q)) q.Enqueue(msg);
                }
            }
            catch { /* peer disconnected / malformed frame */ }
        }

        public static IEnumerable<BusMessage> Drain(SessionData s)
        {
            if (!_queues.TryGetValue(s, out var q)) yield break;
            while (q.TryDequeue(out var m)) yield return m;
        }

        // ---- helpers ----

        private static byte[] ComputeMac(string hexKey, byte[] kind, byte[] ip, byte[] payload)
        {
            using var h = new HMACSHA256(Convert.FromHexString(hexKey));
            h.TransformBlock(kind, 0, kind.Length, null, 0);
            h.TransformBlock(ip, 0, ip.Length, null, 0);
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