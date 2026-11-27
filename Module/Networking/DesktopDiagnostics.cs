using System.Text.Json;
using System.Threading.Channels;
using Module.PacketHandler.Filtering;
using NetGuard.DesktopProtocol;
using SilkroadSecurityAPI;

namespace Module.Networking;

internal sealed class DesktopDiagnostics : IAsyncDisposable
{
    internal static DesktopDiagnostics? Current { get; set; }
    private sealed record Selection(CaptureOptions Options, HashSet<ushort> Ignore, HashSet<ushort> Only);
    private Selection _selection = new(new(), [], []);
    private readonly Channel<(DesktopMessage Message, int Cost)> _queue = Channel.CreateBounded<(DesktopMessage, int)>(
        new BoundedChannelOptions(256) { SingleReader = true, FullMode = BoundedChannelFullMode.Wait, AllowSynchronousContinuations = false });
    private readonly System.Collections.Concurrent.ConcurrentDictionary<int, Func<Packet, bool, CancellationToken, Task>> _connections = new();
    internal void Register(int id, Func<Packet, bool, CancellationToken, Task> inject) => _connections[id] = inject;
    internal void Unregister(int id) => _connections.TryRemove(id, out _);
    private readonly int _module;
    private readonly TextWriter _output;
    private readonly Task _worker;
    private long _pendingBytes, _dropped;
    private int _stopped;
    internal long Dropped => Interlocked.Read(ref _dropped);
    internal Task Completion => _worker;

    internal DesktopDiagnostics(int module, TextWriter output)
    {
        _module = module; _output = output;
        _worker = Task.Run(DrainAsync);
    }

    internal void Configure(CaptureOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        Volatile.Write(ref _selection, new Selection(options, new(options.Ignore ?? []), new(options.Only ?? [])));
    }

    internal void Capture(Packet packet, string ip, int clientId, bool fromClient, string stage = "received")
    {
        var selected = Volatile.Read(ref _selection);
        var options = selected.Options;
        if (!options.Enabled || Volatile.Read(ref _stopped) != 0
            || (options.Ip != null && !string.Equals(options.Ip, ip, StringComparison.OrdinalIgnoreCase))
            || (fromClient ? !options.ClientToServer : !options.ServerToClient)
            || selected.Ignore.Contains(packet.Opcode)
            || (selected.Only.Count != 0 && !selected.Only.Contains(packet.Opcode))) return;
        int cost = Math.Min(packet.PayloadLength, Protocol.MaximumPayload) + 512;
        if (!Reserve(cost)) return;
        try
        {
            var message = new DesktopMessage
            {
                Stage = stage, Module = _module, Ip = ip, ClientId = clientId, FromClient = fromClient,
                Opcode = packet.Opcode, Payload = packet.CopyPayload(Protocol.MaximumPayload), Length = packet.PayloadLength,
                Encrypted = packet.Encrypted, Massive = packet.Massive
            };
            if (_queue.Writer.TryWrite((message, cost))) return;
            Interlocked.Increment(ref _dropped);
        }
        catch { Interlocked.Increment(ref _dropped); }
        Interlocked.Add(ref _pendingBytes, -cost);
    }

    internal void Log(string text, ConsoleColor color)
    {
        if (Volatile.Read(ref _stopped) != 0) return;
        if (text.Length > 8192) text = text[..8192] + " [log line truncated]";
        int cost = text.Length * 2 + 512;
        if (!Reserve(cost)) return;
        if (!_queue.Writer.TryWrite((new DesktopMessage { Type = "log", Module = _module, Text = $"[{color}] {text}" }, cost)))
        { Interlocked.Add(ref _pendingBytes, -cost); Interlocked.Increment(ref _dropped); }
    }

    private bool Reserve(int cost)
    {
        if (Interlocked.Add(ref _pendingBytes, cost) <= 8 * 1024 * 1024) return true;
        Interlocked.Add(ref _pendingBytes, -cost);
        Interlocked.Increment(ref _dropped);
        return false;
    }

    internal async Task HandleCommandAsync(string json)
    {
        try
        {
            if (json.Length > 100000) throw new InvalidDataException("Desktop command too long.");
            var command = JsonSerializer.Deserialize<DesktopCommand>(json, Protocol.Json);
            if (command?.Type == "inject" && command.Injection is { } request)
            {
                string result;
                try
                {
                    if (request.Opcode is 0x5000 or 0x9000 or 0x600D)
                        throw new InvalidDataException("Handshake and fragmentation-control opcodes are reserved.");
                    if (request.Payload == null || request.Payload.Length > 32767 || (request.Massive && request.Encrypted)
                        || (request.Massive && request.Payload.Length == 0))
                        throw new InvalidDataException("Invalid payload size or encryption/massive combination.");
                    if (!_connections.TryGetValue(request.ClientId, out var inject))
                        throw new InvalidDataException("The selected connection is no longer available.");
                    var packet = new Packet(request.Opcode, request.Encrypted, request.Massive, request.Payload);
                    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    await inject(packet, request.ToServer, timeout.Token);
                    if (FilterRegistry.LiveSessions.TryGetValue(request.ClientId, out var session))
                        Capture(packet, session.PlayerInfo.IpInfo.Ip, request.ClientId, request.ToServer, "injected");
                    result = $"Injected 0x{request.Opcode:X4} ({request.Payload.Length} bytes) to {(request.ToServer ? "Remote/server" : "Local/client")}, client {request.ClientId}.";
                }
                catch (Exception ex) { result = "Injection not confirmed: " + ex.Message; }
                const int responseCost = 2048;
                // Control replies may wait briefly on the console thread, never on a packet thread.
                Interlocked.Add(ref _pendingBytes, responseCost);
                using var deliveryDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                try { await _queue.Writer.WriteAsync((new DesktopMessage { Type = "injection", Module = _module, Text = result }, responseCost), deliveryDeadline.Token); }
                catch (Exception ex) when (ex is OperationCanceledException or ChannelClosedException)
                { Interlocked.Add(ref _pendingBytes, -responseCost); Interlocked.Increment(ref _dropped); }
                return;
            }
            if (command?.Type == "capture" && command.Capture != null) Configure(command.Capture);
            else if (command?.Type != "status") return;
            var message = new DesktopMessage
            {
                Type = "status", Module = _module, Dropped = Dropped,
                Text = NetworkMetrics.DesktopSnapshot(),
                Peers = FilterRegistry.LiveSessions.Select(p => new PeerInfo(p.Key, p.Value.PlayerInfo.IpInfo.Ip)).ToArray()
            };
            int cost = 1024 + (message.Peers.Length * 128);
            if (!Reserve(cost)) return;
            if (!_queue.Writer.TryWrite((message, cost))) { Interlocked.Add(ref _pendingBytes, -cost); Interlocked.Increment(ref _dropped); }
        }
        catch (Exception ex) when (ex is JsonException or InvalidDataException)
        { Services.Custom.WriteLine("[desktop] " + ex.Message, ConsoleColor.Yellow); }
    }

    private async Task DrainAsync()
    {
        try
        {
            await foreach (var entry in _queue.Reader.ReadAllAsync())
            {
                try { await _output.WriteLineAsync(Protocol.Prefix + JsonSerializer.Serialize(entry.Message, Protocol.Json)); }
                finally { Interlocked.Add(ref _pendingBytes, -entry.Cost); }
            }
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException) { }
        finally
        {
            Volatile.Write(ref _stopped, 1);
            _queue.Writer.TryComplete();
            while (_queue.Reader.TryRead(out var entry)) Interlocked.Add(ref _pendingBytes, -entry.Cost);
        }
    }

    public async ValueTask DisposeAsync()
    {
        Volatile.Write(ref _stopped, 1);
        _queue.Writer.TryComplete();
        await _worker;
    }
}
