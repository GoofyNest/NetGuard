using System.Buffers;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Module.Networking;

// Owns only completed wire bytes, never Security or mutable session state.
internal sealed class OrderedSocketSender
{
    internal const int BatchBytes = 64 * 1024;
    internal const int MaxPendingBytes = 8 * 1024 * 1024;
    private readonly record struct Write(ReadOnlyMemory<byte> Bytes, TaskCompletionSource? Receipt = null);
    private readonly Channel<Write> _queue = Channel.CreateBounded<Write>(
        new BoundedChannelOptions(4096) { SingleReader = true, FullMode = BoundedChannelFullMode.Wait });
    private readonly CancellationTokenSource _stop = new();
    private int _pendingBytes;
    internal Task Completion { get; }
    internal int PendingBytes => Math.Max(0, Volatile.Read(ref _pendingBytes));

    internal OrderedSocketSender(Socket socket, Action<Exception> failed)
    {
        NetworkMetrics.Register(this);
        Completion = RunAsync(socket, failed);
    }

    // Caller transfers ownership: backing bytes must not be changed afterwards.
    internal bool TryEnqueue(ReadOnlyMemory<byte> bytes)
    {
        if (bytes.IsEmpty) return true;
        if (bytes.Length > BatchBytes) throw new ArgumentOutOfRangeException(nameof(bytes));
        if (Interlocked.Add(ref _pendingBytes, bytes.Length) > MaxPendingBytes)
        {
            Interlocked.Add(ref _pendingBytes, -bytes.Length);
            return false;
        }
        if (_queue.Writer.TryWrite(new Write(bytes))) return true;
        Interlocked.Add(ref _pendingBytes, -bytes.Length);
        return false;
    }

    internal void Stop()
    {
        _queue.Writer.TryComplete();
        _stop.Cancel();
    }

    // A FIFO barrier completes only after all preceding bytes reach the socket.
    internal Task FlushAsync()
    {
        var receipt = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_queue.Writer.TryWrite(new Write(default, receipt)))
            receipt.TrySetException(new IOException("Socket writer queue is full or closed."));
        return receipt.Task;
    }

    internal Task FinishAsync()
    {
        _queue.Writer.TryComplete();
        return Completion;
    }

    internal static async Task DrainAsync(IEnumerable<OrderedSocketSender> senders, TimeSpan timeout)
    {
        var writers = senders.ToArray();
        try { await Task.WhenAll(writers.Select(s => s.FinishAsync())).WaitAsync(timeout); }
        catch
        {
            foreach (var writer in writers) writer.Stop();
            throw;
        }
    }

    private async Task RunAsync(Socket socket, Action<Exception> failed)
    {
        byte[]? batch = null;
        try
        {
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(_stop.Token);
            while (await _queue.Reader.WaitToReadAsync(_stop.Token))
            {
                if (!_queue.Reader.TryRead(out var first)) continue;
                if (first.Receipt != null) { first.Receipt.TrySetResult(); continue; }
                int size = first.Bytes.Length;
                ReadOnlyMemory<byte> output = first.Bytes;
                if (_queue.Reader.TryPeek(out var next) && next.Receipt == null && size + next.Bytes.Length <= BatchBytes)
                {
                    batch ??= ArrayPool<byte>.Shared.Rent(BatchBytes);
                    first.Bytes.CopyTo(batch);
                    while (_queue.Reader.TryPeek(out next) && next.Receipt == null && size + next.Bytes.Length <= BatchBytes)
                    {
                        if (!_queue.Reader.TryRead(out next)) break;
                        next.Bytes.CopyTo(batch.AsMemory(size));
                        size += next.Bytes.Length;
                    }
                    output = batch.AsMemory(0, size);
                }
                deadline.CancelAfter(TimeSpan.FromSeconds(30));
                await SocketIO.SendAllAsync(socket, output, deadline.Token);
                deadline.CancelAfter(Timeout.InfiniteTimeSpan);
                Interlocked.Add(ref _pendingBytes, -size);
                NetworkMetrics.Sent(size);
                if (batch != null)
                {
                    ArrayPool<byte>.Shared.Return(batch);
                    batch = null;
                }
            }
        }
        catch (OperationCanceledException) when (_stop.IsCancellationRequested) { }
        catch (Exception ex) { failed(ex); }
        finally
        {
            NetworkMetrics.Unregister(this);
            _queue.Writer.TryComplete();
            while (_queue.Reader.TryRead(out var write)) write.Receipt?.TrySetCanceled();
            if (batch != null) ArrayPool<byte>.Shared.Return(batch);
        }
    }
}
