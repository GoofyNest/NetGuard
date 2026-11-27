using System.Threading.Channels;
using Module.PacketHandler;
using SilkroadSecurityAPI;

namespace Module.Networking;

// Only opted-in ModifiedPackets lists pass through this queue. Ordinary proxy
// traffic continues independently. The sender registers the wait while holding
// the connection's processing gate, immediately before queuing the packet.
internal sealed class ScriptedPacketSequence
{
    private sealed record Step(Packet Packet, bool ToServer, ushort? Reply, int TimeoutMs);
    private sealed record ReplyWait(ushort Opcode, TaskCompletionSource Completion);
    private sealed record Batch(Step[] Steps, long Bytes);
    internal const long MaxPendingBytes = 8 * 1024 * 1024;
    private long _pendingBytes;
    private readonly Channel<Batch> _queue = Channel.CreateBounded<Batch>(new BoundedChannelOptions(32)
    { SingleReader = true, FullMode = BoundedChannelFullMode.Wait, AllowSynchronousContinuations = false });
    private readonly Func<Packet, bool, Action, CancellationToken, Task> _send;
    private readonly Action<string> _log;
    private readonly CancellationTokenSource _stop = new();
    private ReplyWait? _waiting;
    private int _stopped;
    internal Task Completion { get; }

    internal ScriptedPacketSequence(Func<Packet, bool, Action, CancellationToken, Task> send, Action<string> log)
    {
        _send = send; _log = log;
        Completion = RunAsync();
    }

    internal void Enqueue(IReadOnlyList<PacketList> packets, bool originalFromClient)
    {
        if (Volatile.Read(ref _stopped) != 0) return;
        if (packets.Count > 256) { _log("Rejected sequence: more than 256 steps."); return; }
        long bytes = 0;
        foreach (var entry in packets)
        {
            if (entry?.Packet == null) { _log("Rejected sequence: missing packet."); return; }
            bytes += entry.Packet.PayloadLength + 64L;
        }
        if (Interlocked.Add(ref _pendingBytes, bytes) > MaxPendingBytes)
        {
            Interlocked.Add(ref _pendingBytes, -bytes);
            _log("Rejected sequence: scripting byte limit exceeded."); return;
        }
        bool queued = false;
        try
        {
            var steps = new Step[packets.Count];
            for (int i = 0; i < packets.Count; i++)
            {
                var entry = packets[i];
                bool toServer = entry.SecurityType switch
                {
                    SecurityType.RemoteSecurity => true,
                    SecurityType.LocalSecurity => false,
                    SecurityType.Default => originalFromClient,
                    _ => throw new ArgumentOutOfRangeException(nameof(entry.SecurityType))
                };
                if (entry.Packet == null || entry.Packet.Opcode is 0x5000 or 0x9000
                    || (entry.ServerReply.HasValue && (!toServer || entry.ServerReplyTimeoutMs <= 0)))
                {
                    _log($"Rejected sequence: invalid packet, direction or reply timeout at step {i + 1}.");
                    return;
                }
                var packet = new Packet(entry.Packet);
                packet.Lock();
                steps[i] = new Step(packet, toServer, entry.ServerReply, entry.ServerReplyTimeoutMs);
            }
            queued = _queue.Writer.TryWrite(new Batch(steps, bytes));
            if (!queued) _log("Rejected sequence: scripting queue is full or closed.");
        }
        finally { if (!queued) Interlocked.Add(ref _pendingBytes, -bytes); }
    }

    internal void ObserveServerPacket(ushort opcode)
    {
        var wait = Volatile.Read(ref _waiting);
        if (wait?.Opcode == opcode && ReferenceEquals(Interlocked.CompareExchange(ref _waiting, null, wait), wait))
            wait.Completion.TrySetResult();
    }

    internal void Stop()
    {
        if (Interlocked.Exchange(ref _stopped, 1) != 0) return;
        _queue.Writer.TryComplete();
        _stop.Cancel();
    }

    private async Task RunAsync()
    {
        try
        {
            await foreach (var batch in _queue.Reader.ReadAllAsync(_stop.Token))
            {
                try
                {
                    foreach (var step in batch.Steps)
                    {
                        var wait = step.Reply is { } opcode
                            ? new ReplyWait(opcode, new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)) : null;
                        try
                        {
                            await _send(step.Packet, step.ToServer, () => Volatile.Write(ref _waiting, wait), _stop.Token);
                            if (wait != null)
                                await wait.Completion.Task.WaitAsync(TimeSpan.FromMilliseconds(step.TimeoutMs), _stop.Token);
                        }
                        catch (TimeoutException)
                        {
                            _log($"Timeout: sent 0x{step.Packet.Opcode:X4}, waiting for server 0x{step.Reply:X4} ({step.TimeoutMs}ms). Remaining steps cancelled.");
                            break;
                        }
                        catch (OperationCanceledException) when (_stop.IsCancellationRequested) { throw; }
                        catch (Exception ex)
                        {
                            _log($"Sequence cancelled at 0x{step.Packet.Opcode:X4}: {ex.Message}");
                            break;
                        }
                        finally
                        {
                            if (wait != null) Interlocked.CompareExchange(ref _waiting, null, wait);
                        }
                    }
                }
                finally { Interlocked.Add(ref _pendingBytes, -batch.Bytes); }
            }
        }
        catch (OperationCanceledException) when (_stop.IsCancellationRequested) { }
        finally
        {
            Interlocked.Exchange(ref _waiting, null);
            while (_queue.Reader.TryRead(out var batch)) Interlocked.Add(ref _pendingBytes, -batch.Bytes);
        }
    }
}
