using System.Diagnostics;

namespace Module.Networking;

internal sealed class ByteRateLimiter
{
    private readonly double _bytesPerSecond;
    private readonly double _burstBytes;
    private double _tokens;
    private long _last = Stopwatch.GetTimestamp();
    public ByteRateLimiter(double bytesPerSecond, double burstBytes)
    {
        _bytesPerSecond = bytesPerSecond;
        _burstBytes = burstBytes;
        _tokens = burstBytes;
    }

    // Called by the connection's serialized processing loop.
    public bool TryConsume(int bytes)
    {
        long now = Stopwatch.GetTimestamp();
        _tokens = Math.Min(_burstBytes, _tokens + Stopwatch.GetElapsedTime(_last, now).TotalSeconds * _bytesPerSecond);
        _last = now;
        if (bytes > _tokens) return false;
        _tokens -= bytes;
        return true;
    }
}
