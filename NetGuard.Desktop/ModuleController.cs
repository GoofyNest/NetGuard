using System.Diagnostics;
using System.Text;
using System.Text.Json;
using NetGuard.DesktopProtocol;

namespace NetGuard.Desktop;

internal sealed record ModuleDefinition(int Index, string Name, string Kind, string Listen);
internal sealed record UiEvent(int Module, DesktopMessage? Message = null, string? Log = null, string? State = null);

internal sealed class ModuleController(ModuleDefinition definition, Action<UiEvent> receive)
{
    internal ModuleDefinition Definition { get; } = definition;
    private Process? _process;
    private readonly SemaphoreSlim _input = new(1);
    internal bool Running => _process is { HasExited: false };

    internal void Start()
    {
        if (Running) return;
        _process?.Dispose();
        string executable = Path.Combine(AppContext.BaseDirectory, "NetGuardLoader.exe");
        if (!File.Exists(executable)) throw new FileNotFoundException("Build the full solution first; NetGuardLoader.exe is missing.");
        var info = new ProcessStartInfo(executable)
        {
            WorkingDirectory = AppContext.BaseDirectory, UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true,
            StandardInputEncoding = new UTF8Encoding(false), StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8
        };
        info.ArgumentList.Add("--desktop"); info.ArgumentList.Add(Definition.Index.ToString());
        var process = Process.Start(info) ?? throw new IOException("Cannot start module.");
        _process = process;
        receive(new(Definition.Index, State: "Starting"));
        _ = ObserveAsync(process);
    }

    private async Task ObserveAsync(Process process)
    {
        async Task ReadAsync(StreamReader reader)
        {
            try
            {
                while (await reader.ReadLineAsync() is { } line)
                {
                    if (line.StartsWith(Protocol.Prefix, StringComparison.Ordinal))
                    {
                        try
                        {
                            var message = JsonSerializer.Deserialize<DesktopMessage>(line.AsSpan(Protocol.Prefix.Length), Protocol.Json);
                            if (message != null) receive(new(Definition.Index, Message: message));
                        }
                        catch (JsonException) { receive(new(Definition.Index, Log: "Invalid diagnostic message from worker.")); }
                    }
                    else if (line.Length != 0) receive(new(Definition.Index, Log: line[..Math.Min(line.Length, 8192)]));
                }
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            { receive(new(Definition.Index, Log: ex.Message)); }
        }
        await Task.WhenAll(ReadAsync(process.StandardOutput), ReadAsync(process.StandardError));
        await process.WaitForExitAsync();
        receive(new(Definition.Index, State: process.ExitCode == 0 ? "Stopped" : $"Exited ({process.ExitCode})"));
    }

    internal async Task SendAsync(DesktopCommand command) => await WriteAsync(Protocol.Prefix + JsonSerializer.Serialize(command, Protocol.Json));

    private async Task WriteAsync(string text)
    {
        if (!Running) return;
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await _input.WaitAsync(deadline.Token);
        try
        {
            if (Running) { await _process!.StandardInput.WriteLineAsync(text.AsMemory(), deadline.Token); await _process.StandardInput.FlushAsync(deadline.Token); }
        }
        finally { _input.Release(); }
    }

    internal async Task StopAsync()
    {
        var process = _process;
        if (process == null || process.HasExited) return;
        receive(new(Definition.Index, State: "Stopping"));
        await WriteAsync("quit");
        await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(12));
    }
}
