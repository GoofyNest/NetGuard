using SilkroadSecurityAPI;

namespace Module.Networking;

internal sealed class BoundedPacketQueue
{
    internal const int MaxBytes = 8 * 1024 * 1024;
    private readonly Queue<(Packet Packet, int Bytes)> _queue = new();
    private long _bytes;

    internal bool TryEnqueue(Packet packet)
    {
        ArgumentNullException.ThrowIfNull(packet);
        long size = packet.PayloadLength + 64L;
        lock (_queue)
        {
            if (_queue.Count >= 4096 || _bytes + size > MaxBytes) return false;
            var snapshot = new Packet(packet);
            snapshot.Lock();
            _queue.Enqueue((snapshot, (int)size));
            _bytes += size;
            return true;
        }
    }

    internal bool TryDequeue(out Packet packet)
    {
        lock (_queue)
        {
            if (!_queue.TryDequeue(out var entry)) { packet = null!; return false; }
            _bytes -= entry.Bytes;
            packet = entry.Packet;
            return true;
        }
    }
}
