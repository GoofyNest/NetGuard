namespace Module.Services
{
    public static class Custom
    {
        private static readonly object LogLock = new object();

        private static readonly Dictionary<ConsoleColor, string> ColorPrefixes = new Dictionary<ConsoleColor, string>
        {
            { ConsoleColor.Yellow, "[WARN] " },
            { ConsoleColor.DarkYellow, "[WARN] " },
            { ConsoleColor.Magenta, "[DEBUG] " },
            { ConsoleColor.DarkMagenta, "[DEBUG] " },
            { ConsoleColor.Red, "[ERROR] " },
            { ConsoleColor.DarkRed, "[ERROR] " },
            { ConsoleColor.Cyan, "[NOTIFY] " },
            { ConsoleColor.Blue, "[INFO] " },
            { ConsoleColor.DarkBlue, "[INFO] " }
        };

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            string prefix = GetPrefix(color);

            string date = $"[{DateTime.Now:HH:mm:ss}] ";

            if (!Main._settings.disableConsole)
            {
                lock (LogLock) // Ensure thread safety
                {
                    Console.ForegroundColor = color;
                    Console.Write(prefix);
                    Console.ResetColor();

                    // Display timestamp
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(date);
                    Console.ResetColor();

                    Console.WriteLine(message);
                }
            }

            WriteToLog(prefix + date + message);
        }

        private static void WriteToLog(string message)
        {
            if (!Main._settings.logWriting)
                return;

            try
            {
                lock (LogLock) // Ensure thread safety
                {
                    // Use StreamWriter with BufferedStream for better performance
                    using (StreamWriter writer = new StreamWriter(new BufferedStream(File.Open(Main._config.logFile, FileMode.Append, FileAccess.Write))))
                    {
                        writer.WriteLine(message);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        private static string GetPrefix(ConsoleColor color)
        {
            return ColorPrefixes.TryGetValue(color, out string? prefix)
                ? prefix
                : "[NORMAL] ";
        }
    }
}
