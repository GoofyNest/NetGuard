using Module.PacketHandler.Filtering;
using Module.Services;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Module.Networking;

public static class SessionBus
{
    public static int ApiPort { get; set; }
    public static string ApiPassword { get; set; } = "";
    public static string ListenAddress { get; set; } = "127.0.0.1";
    private const int MaxPayload = 1024 * 1024;
    private static readonly TimeSpan FrameTimeout = TimeSpan.FromSeconds(10);
    private static readonly ConcurrentDictionary<SessionData, byte> _sessions = new();
    private static readonly ConcurrentDictionary<(string Host, int Port), Peer> _peers = new();
    private static readonly ConcurrentDictionary<Socket, byte> _incoming = new();
    private static readonly SemaphoreSlim _incomingSlots = new(64, 64);
    private static readonly CancellationTokenSource _stop = new();
    private static TcpListener? _listener;

    private sealed class Peer
    {
        public SemaphoreSlim Gate { get; } = new(1, 1);
        public Socket? Socket;
    }

    public static void StartApiListener()
    {
        if (ApiPort <= 0) throw new InvalidOperationException("SessionBus.ApiPort is not set.");
        if (string.IsNullOrEmpty(ApiPassword) || ApiPassword.Length < 32)
            throw new InvalidOperationException("SessionBus.ApiPassword requires at least 32 hex characters.");
        _ = Convert.FromHexString(ApiPassword); // Validate before accepting connections.
        if (_listener != null) throw new InvalidOperationException("Session bus already started.");
        _listener = new TcpListener(IPAddress.Parse(ListenAddress), ApiPort);
        _listener.Start();
        _ = AcceptLoop(_listener);
        Custom.WriteLine($"[bus] listening on {ListenAddress}:{ApiPort}", ConsoleColor.Cyan);
    }

    private static async Task AcceptLoop(TcpListener listener)
    {
        while (!_stop.IsCancellationRequested)
        {
            try
            {
                var socket = await listener.AcceptSocketAsync(_stop.Token);
                if (!_incomingSlots.Wait(0)) { socket.Dispose(); continue; }
                _incoming[socket] = 0;
                _ = HandleIncoming(socket);
            }
            catch (Exception) when (_stop.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                Custom.WriteLine($"[bus] accept: {ex.Message}", ConsoleColor.Red);
                try { await Task.Delay(100, _stop.Token); } catch (OperationCanceledException) { break; }
            }
        }
    }

    public static void Stop()
    {
        _stop.Cancel();
        _listener?.Stop();
        foreach (var socket in _incoming.Keys) socket.Dispose();
        foreach (var peer in _peers.Values) peer.Socket?.Dispose();
    }

    public static void Register(SessionData session) => _sessions[session] = 0;
    public static void Unregister(SessionData session) => _sessions.TryRemove(session, out _);

    public static void SendTo(string destHost, int destApiPort, string destApiPassword, BusMessage msg)
        => _ = SendToAsync(destHost, destApiPort, destApiPassword, msg);

    private static async Task SendToAsync(string host, int port, string key, BusMessage msg)
    {
        try
        {
            var kind = Encoding.UTF8.GetBytes(msg.Kind);
            var payload = msg.Payload == null ? Array.Empty<byte>() : JsonSerializer.SerializeToUtf8Bytes(msg.Payload);
            if (kind.Length is 0 or > 4096 || payload.Length > MaxPayload)
                throw new InvalidOperationException("Bus message exceeds the frame limits.");
            var mac = ComputeMac(key, kind, payload);
            var frame = new byte[2 + kind.Length + 4 + payload.Length + 2 + mac.Length];
            int offset = 0;
            BitConverter.TryWriteBytes(frame.AsSpan(offset, 2), (ushort)kind.Length); offset += 2;
            kind.CopyTo(frame, offset); offset += kind.Length;
            BitConverter.TryWriteBytes(frame.AsSpan(offset, 4), payload.Length); offset += 4;
            payload.CopyTo(frame, offset); offset += payload.Length;
            BitConverter.TryWriteBytes(frame.AsSpan(offset, 2), (ushort)mac.Length); offset += 2;
            mac.CopyTo(frame, offset);

            var peer = _peers.GetOrAdd((host, port), _ => new Peer());
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(_stop.Token);
            deadline.CancelAfter(FrameTimeout);
            await peer.Gate.WaitAsync(deadline.Token);
            try
            {
                for (int attempt = 0; ; attempt++)
                {
                    try
                    {
                        if (peer.Socket == null)
                        {
                            peer.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                            { NoDelay = true };
                            await peer.Socket.ConnectAsync(host, port, deadline.Token);
                        }
                        await SocketIO.SendAllAsync(peer.Socket, frame, deadline.Token);
                        break;
                    }
                    catch
                    {
                        peer.Socket?.Dispose();
                        peer.Socket = null;
                        if (attempt != 0 || deadline.IsCancellationRequested) throw;
                    }
                }
            }
            finally { peer.Gate.Release(); }
        }
        catch (Exception ex)
        {
            if (!_stop.IsCancellationRequested)
                Custom.WriteLine($"[bus] send to {host}:{port}: {ex.Message}", ConsoleColor.Yellow);
        }
    }

    private static async Task HandleIncoming(Socket socket)
    {
        try
        {
            using (socket)
            using (var stream = new NetworkStream(socket, ownsSocket: false))
            {
                socket.NoDelay = true;
                var header = new byte[4];
                while (!_stop.IsCancellationRequested)
                {
                    // One deadline covers the entire frame, including slow senders.
                    using var deadline = CancellationTokenSource.CreateLinkedTokenSource(_stop.Token);
                    deadline.CancelAfter(FrameTimeout);
                    var token = deadline.Token;
                    if (!await ReadExact(stream, header.AsMemory(0, 2), token)) return;
                    int kindLength = BitConverter.ToUInt16(header);
                    if (kindLength is 0 or > 4096) return;
                    var kind = new byte[kindLength];
                    if (!await ReadExact(stream, kind, token)) return;
                    if (!await ReadExact(stream, header, token)) return;
                    int payloadLength = BitConverter.ToInt32(header);
                    if (payloadLength < 0 || payloadLength > MaxPayload) return;
                    var payload = new byte[payloadLength];
                    if (!await ReadExact(stream, payload, token)) return;
                    if (!await ReadExact(stream, header.AsMemory(0, 2), token)) return;
                    if (BitConverter.ToUInt16(header) != 32) return;
                    var mac = new byte[32];
                    if (!await ReadExact(stream, mac, token)) return;
                    if (!CryptographicOperations.FixedTimeEquals(ComputeMac(ApiPassword, kind, payload), mac))
                        return;
                    object? body = payload.Length == 0 ? null : JsonSerializer.Deserialize<JsonElement>(payload);
                    var msg = new BusMessage(Encoding.UTF8.GetString(kind), body);
                    foreach (var session in _sessions.Keys)
                        BusRegistry.Dispatcher.Dispatch(session, msg);
                }
            }
        }
        catch (Exception ex) when (ex is IOException or SocketException or OperationCanceledException
            or ObjectDisposedException or JsonException) { }
        finally
        {
            socket.Dispose();
            _incoming.TryRemove(socket, out _);
            _incomingSlots.Release();
        }
    }

    private static byte[] ComputeMac(string hexKey, byte[] kind, byte[] payload)
    {
        using var h = new HMACSHA256(Convert.FromHexString(hexKey));
        h.TransformBlock(kind, 0, kind.Length, null, 0);
        h.TransformFinalBlock(payload, 0, payload.Length);
        return h.Hash!;
    }

    private static async Task<bool> ReadExact(NetworkStream stream, Memory<byte> buffer, CancellationToken token)
    {
        while (!buffer.IsEmpty)
        {
            int count = await stream.ReadAsync(buffer, token);
            if (count == 0) return false;
            buffer = buffer[count..];
        }
        return true;
    }
}
