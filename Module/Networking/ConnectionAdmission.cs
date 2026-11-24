namespace Module.Networking;

internal sealed class ConnectionAdmission(int maximum, int perIp)
{
    private readonly object _sync = new();
    private readonly Dictionary<string, int> _counts = new(StringComparer.Ordinal);
    private int _total;

    internal bool TryAcquire(string ip)
    {
        lock (_sync)
        {
            _counts.TryGetValue(ip, out int count);
            if (_total >= maximum || count >= perIp) return false;
            _counts[ip] = count + 1;
            _total++;
            return true;
        }
    }

    internal void Release(string ip)
    {
        lock (_sync)
        {
            if (!_counts.TryGetValue(ip, out int count)) return;
            if (count == 1) _counts.Remove(ip); else _counts[ip] = count - 1;
            _total--;
        }
    }
}
