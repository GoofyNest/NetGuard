using System.Collections.Concurrent;
using System.IO;
using System.Threading.Channels;

namespace Module.Services
{
    public static class Custom
    {
        private static readonly Dictionary<ConsoleColor, string> ColorPrefixes = new()
        {
            { ConsoleColor.Yellow,     "[WARN] " },
            { ConsoleColor.DarkYellow, "[WARN] " },
            { ConsoleColor.DarkMagenta,"[DEBUG] " },
            { ConsoleColor.Magenta,    "[DEBUG] " },
            { ConsoleColor.Red,        "[ERROR] " },
            { ConsoleColor.DarkRed,    "[ERROR] " },
            { ConsoleColor.Cyan,       "[NOTIFY] " },
            { ConsoleColor.Blue,       "[INFO] " },
            { ConsoleColor.DarkBlue,   "[INFO] " }
        };

        public static bool ConsoleEnabled { get; set; } = true;
        private static long _droppedLogEntries;
        private static long _droppedConsoleEntries;
        public static long DroppedLogEntries => Interlocked.Read(ref _droppedLogEntries);
        public static long DroppedConsoleEntries => Interlocked.Read(ref _droppedConsoleEntries);

        private readonly record struct LogEntry(
            string Message, ConsoleColor Color, bool ToFile, DateTime When);

        private static readonly Channel<LogEntry> _channel =
            Channel.CreateBounded<LogEntry>(new BoundedChannelOptions(8192)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.DropWrite,
                AllowSynchronousContinuations = false
            }, _ => Interlocked.Increment(ref _droppedLogEntries));

        // Selection can block console writes. Keep them off the log drain and
        // networking thread pool; bound the backlog while selection is held.
        private static readonly Channel<(string Text, ConsoleColor Color)> _consoleChannel =
            Channel.CreateBounded<(string Text, ConsoleColor Color)>(new BoundedChannelOptions(8192)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropOldest,
                AllowSynchronousContinuations = false
            }, _ => Interlocked.Increment(ref _droppedConsoleEntries));
        private static int _started;
        private static Task _drainTask = Task.CompletedTask;

        private static StreamWriter? _fileWriter;
        private static readonly Lock _fileInitLock = new();
        private static bool _fileInitAttempted;
        private static int _linesSinceFlush;

        public static void StartDrain()
        {
            if (Interlocked.Exchange(ref _started, 1) != 0) return;
            AppDomain.CurrentDomain.UnhandledException += (_, _) => FlushAndStop();
            new Thread(DrainConsole)
            {
                IsBackground = true,
                Name = "console-output"
            }.Start();
            _drainTask = Task.Run(DrainLoopAsync);
        }

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.Green)
        {
            Networking.DesktopDiagnostics.Current?.Log(message, color);
            _channel.Writer.TryWrite(new LogEntry(message, color, true, DateTime.Now));
        }

        public static void FlushAndStop()
        {
            // ProcessExit cannot await indefinitely; the drain owns file disposal.
            try { FlushAndStopAsync().Wait(TimeSpan.FromSeconds(5)); } catch { }
        }

        public static Task FlushAndStopAsync()
        {
            _channel.Writer.TryComplete();
            return _drainTask;
        }

        private static async Task DrainLoopAsync()
        {
            try
            {
                while (true)
                {
                    using var flushDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                    try
                    {
                        if (!await _channel.Reader.WaitToReadAsync(flushDeadline.Token)) break;
                    }
                    catch (OperationCanceledException)
                    {
                        try { _fileWriter?.Flush(); } catch { }
                        continue;
                    }

                    while (_channel.Reader.TryRead(out var entry))
                    {
                    string formatted = Format(entry);
                    if (ConsoleEnabled && !Main.Settings.DisableConsoleLogging && Networking.DesktopDiagnostics.Current == null)
                        _consoleChannel.Writer.TryWrite((formatted, entry.Color));

                    if (entry.ToFile)
                        WriteToFile(formatted);

                    if (++_linesSinceFlush >= 100)
                    {
                        _linesSinceFlush = 0;
                        try { _fileWriter?.Flush(); } catch { }
                    }
                    }
                }
            }
            catch { }
            finally
            {
                _consoleChannel.Writer.TryComplete();
                try { _fileWriter?.Flush(); _fileWriter?.Dispose(); } catch { }
                _fileWriter = null;
            }
        }

        private static void DrainConsole()
        {
            ConsoleColor lastColor = (ConsoleColor)(-1);
            try
            {
                while (_consoleChannel.Reader.WaitToReadAsync().AsTask().GetAwaiter().GetResult())
                {
                    while (_consoleChannel.Reader.TryRead(out var entry))
                    {
                        try
                        {
                            if (entry.Color != lastColor)
                            {
                                Console.ForegroundColor = entry.Color;
                                lastColor = entry.Color;
                            }
                            Console.WriteLine(entry.Text);
                        }
                        catch { }
                    }
                }
            }
            finally
            {
                try { Console.ResetColor(); } catch { }
            }
        }

        private static string Format(in LogEntry entry)
        {
            string prefix = ColorPrefixes.TryGetValue(entry.Color, out var p) ? p : "[NORMAL] ";
            return $"{prefix}[{entry.When:HH:mm:ss}] {entry.Message}";
        }

        private static void WriteToFile(string message)
        {
            if (Main.Settings.DisableLogWriting) return;

            try
            {
                if (_fileWriter == null && !_fileInitAttempted)
                {
                    var path = Main.Config.LogFile;
                    if (!string.IsNullOrEmpty(path))
                    {
                        lock (_fileInitLock)
                        {
                            if (_fileWriter == null)
                            {
                                var fs = new FileStream(
                                    path,
                                    FileMode.Append,
                                    FileAccess.Write,
                                    FileShare.ReadWrite,
                                    bufferSize: 65536,
                                    useAsync: false);
                                _fileWriter = new StreamWriter(fs) { AutoFlush = false };
                                _fileInitAttempted = true;   // ← only set true on success
                            }
                        }
                    }
                }

                _fileWriter?.WriteLine(message);
            }
            catch { }
        }
    }
}
