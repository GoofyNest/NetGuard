using System.Text;
using System.Threading.Channels;
using NetGuard.DesktopProtocol;

namespace NetGuard.Desktop;

internal sealed class PacketFileLog : IAsyncDisposable
{
    private readonly Channel<DesktopMessage> _queue = Channel.CreateBounded<DesktopMessage>(new BoundedChannelOptions(128)
        { SingleReader = true, FullMode = BoundedChannelFullMode.Wait, AllowSynchronousContinuations = false });
    private readonly Task _worker;
    internal string DirectoryPath { get; } = Path.Combine(AppContext.BaseDirectory, "log", "packets");
    private long _dropped;
    internal long Dropped => Interlocked.Read(ref _dropped);
    internal string? Error { get; private set; }

    internal PacketFileLog() => _worker = Task.Run(DrainAsync);
    internal void Enqueue(DesktopMessage packet) { if (!_queue.Writer.TryWrite(packet)) Interlocked.Increment(ref _dropped); }

    private async Task DrainAsync()
    {
        StreamWriter? writer = null;
        long bytes = 0;
        int part = 0;
        string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        try
        {
            await foreach (var packet in _queue.Reader.ReadAllAsync())
            {
                if (writer == null || bytes > 10 * 1024 * 1024)
                {
                    if (writer != null) await writer.DisposeAsync();
                    Directory.CreateDirectory(DirectoryPath);
                    // Five rotating parts bound disk use for this capture session.
                    string path = Path.Combine(DirectoryPath, $"capture-{stamp}-{Environment.ProcessId}-{part++ % 5}.txt");
                    writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read), new UTF8Encoding(false));
                    bytes = 0;
                }
                string text = PacketFormatter.Format(packet);
                await writer.WriteLineAsync(text);
                await writer.FlushAsync();
                bytes += Encoding.UTF8.GetByteCount(text) + 2;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { Error = ex.Message; }
        finally
        {
            _queue.Writer.TryComplete();
            while (_queue.Reader.TryRead(out _)) Interlocked.Increment(ref _dropped);
            if (writer != null) await writer.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync() { _queue.Writer.TryComplete(); await _worker; }
}
