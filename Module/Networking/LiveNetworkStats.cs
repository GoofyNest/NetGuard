using Module.PacketHandler.Filtering;
using Module.Services;

namespace Module.Networking;

// One small, replaceable snapshot per process; no packet payloads or player data.
internal sealed class LiveNetworkStats : IAsyncDisposable
{
    private readonly CancellationTokenSource _stop = new();
    private readonly Task _worker;

    internal LiveNetworkStats(string path, TimeSpan? interval = null)
    {
        string fullPath = Path.GetFullPath(path);
        _worker = Task.Run(async () =>
        {
            using var timer = new PeriodicTimer(interval ?? TimeSpan.FromSeconds(10));
            bool reportedFailure = false;
            try
            {
                do
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                        string temporary = fullPath + "." + Environment.ProcessId + ".tmp";
                        string snapshot = $"{DateTimeOffset.Now:O} pid={Environment.ProcessId} live={FilterRegistry.LiveSessions.Count} "
                            + NetworkMetrics.LiveSnapshot() + Environment.NewLine;
                        await File.WriteAllTextAsync(temporary, snapshot, _stop.Token);
                        File.Move(temporary, fullPath, overwrite: true);
                        reportedFailure = false;
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                    {
                        if (!reportedFailure) Custom.WriteLine($"[stats] Cannot write live snapshot: {ex.Message}", ConsoleColor.Yellow);
                        reportedFailure = true;
                    }
                } while (await timer.WaitForNextTickAsync(_stop.Token));
            }
            catch (OperationCanceledException) when (_stop.IsCancellationRequested) { }
        });
    }

    public async ValueTask DisposeAsync()
    {
        _stop.Cancel();
        await _worker;
    }
}
