using System.Collections.Concurrent;
using System.Diagnostics;

namespace Module.Networking;

internal static class NetworkMetrics
{
    private static long _receivedBytes, _sentBytes, _packets, _filterTicks, _filterCalls;
    private static long _gateTicks, _gateCalls, _formatTicks, _formatCalls;
    private static readonly ConcurrentDictionary<string, long> Disconnects = new();
    private static readonly ConcurrentDictionary<OrderedSocketSender, byte> Writers = new();
    private static readonly object SnapshotLock = new();
    private sealed class RateSample
    {
        internal long LastSnapshot = Stopwatch.GetTimestamp(), Packets, Received, Sent;
    }
    private static readonly RateSample ConsoleSample = new(), LiveSample = new(), DesktopSample = new();

    internal static void Received(int bytes) => Interlocked.Add(ref _receivedBytes, bytes);
    internal static void Sent(int bytes) => Interlocked.Add(ref _sentBytes, bytes);
    internal static void Packet() => Interlocked.Increment(ref _packets);
    internal static void Filter(long started) { Interlocked.Add(ref _filterTicks, Stopwatch.GetTimestamp() - started); Interlocked.Increment(ref _filterCalls); }
    internal static void Gate(long started) { Interlocked.Add(ref _gateTicks, Stopwatch.GetTimestamp() - started); Interlocked.Increment(ref _gateCalls); }
    internal static void Format(long started) { Interlocked.Add(ref _formatTicks, Stopwatch.GetTimestamp() - started); Interlocked.Increment(ref _formatCalls); }
    internal static void Disconnected(string reason) => Disconnects.AddOrUpdate(reason, 1, static (_, count) => count + 1);
    internal static void Register(OrderedSocketSender writer) => Writers.TryAdd(writer, 0);
    internal static void Unregister(OrderedSocketSender writer) => Writers.TryRemove(writer, out _);

    internal static string Snapshot() => Snapshot(ConsoleSample);
    internal static string DesktopSnapshot() => Snapshot(DesktopSample);
    internal static string LiveSnapshot() => Snapshot(LiveSample);

    private static string Snapshot(RateSample sample)
    {
        lock (SnapshotLock)
        {
            long now = Stopwatch.GetTimestamp(), packets = Interlocked.Read(ref _packets);
            long received = Interlocked.Read(ref _receivedBytes), sent = Interlocked.Read(ref _sentBytes);
            double seconds = Math.Max((now - sample.LastSnapshot) / (double)Stopwatch.Frequency, 0.001);
            static double MeanUs(long ticks, long calls) => ticks * 1_000_000d / Stopwatch.Frequency / Math.Max(calls, 1);
            string result = $"[network] packets/s={(packets - sample.Packets) / seconds:F0} "
                + $"recvKiB/s={(received - sample.Received) / seconds / 1024:F1} sentKiB/s={(sent - sample.Sent) / seconds / 1024:F1} "
                + $"socketQueuedKiB={Writers.Keys.Sum(w => (long)w.PendingBytes) / 1024d:F1} "
                + $"meanFilterUs={MeanUs(Interlocked.Read(ref _filterTicks), Interlocked.Read(ref _filterCalls)):F1} "
                + $"meanGateWaitUs={MeanUs(Interlocked.Read(ref _gateTicks), Interlocked.Read(ref _gateCalls)):F1} "
                + $"meanFormatBatchUs={MeanUs(Interlocked.Read(ref _formatTicks), Interlocked.Read(ref _formatCalls)):F1} "
                + $"disconnects=[{string.Join(", ", Disconnects.OrderBy(p => p.Key).Select(p => $"{p.Key}:{p.Value}"))}]";
            sample.LastSnapshot = now; sample.Packets = packets; sample.Received = received; sample.Sent = sent;
            return result;
        }
    }
}
