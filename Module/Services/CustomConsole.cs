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

        private readonly record struct LogEntry(
            string Message, ConsoleColor Color, bool ToFile, DateTime When);

        private static readonly Channel<LogEntry> _channel =
            Channel.CreateBounded<LogEntry>(new BoundedChannelOptions(8192)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.DropWrite,
                AllowSynchronousContinuations = false
            });

        private static StreamWriter? _fileWriter;
        private static readonly object _fileInitLock = new();
        private static bool _fileInitAttempted;
        private static int _linesSinceFlush;

        public static void StartDrain()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => FlushAndStop();
            _ = Task.Run(DrainLoopAsync);
        }

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.Green)
        {
            _channel.Writer.TryWrite(new LogEntry(message, color, true, DateTime.Now));
        }

        public static void FlushAndStop()
        {
            try { _channel.Writer.TryComplete(); } catch { }
            try { _fileWriter?.Flush(); } catch { }
            try { _fileWriter?.Dispose(); } catch { }
            _fileWriter = null;
        }

        private static async Task DrainLoopAsync()
        {
            ConsoleColor lastColor = (ConsoleColor)(-1);

            try
            {
                await foreach (var entry in _channel.Reader.ReadAllAsync())
                {
                    string formatted = Format(entry);

                    if (ConsoleEnabled)
                    {
                        try
                        {
                            if (entry.Color != lastColor)
                            {
                                Console.ForegroundColor = entry.Color;
                                lastColor = entry.Color;
                            }
                            Console.WriteLine(formatted);
                        }
                        catch { }
                    }

                    if (entry.ToFile)
                        WriteToFile(formatted);

                    if (++_linesSinceFlush >= 100)
                    {
                        _linesSinceFlush = 0;
                        try { _fileWriter?.Flush(); } catch { }
                    }
                }
            }
            catch { }
            finally
            {
                try { if (ConsoleEnabled) Console.ResetColor(); } catch { }
                try { _fileWriter?.Flush(); _fileWriter?.Dispose(); } catch { }
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