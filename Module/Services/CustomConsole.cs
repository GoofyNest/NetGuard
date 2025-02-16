namespace Module.Services
{
    public static class Custom
    {
        private static readonly Dictionary<ConsoleColor, string> ColorPrefixes = new()
        {
            { ConsoleColor.Yellow, "[WARN] " },
            { ConsoleColor.DarkYellow, "[WARN] " },
            { ConsoleColor.DarkMagenta, "[DEBUG] " },
            { ConsoleColor.Magenta, "[DEBUG] " },
            { ConsoleColor.Red, "[ERROR] " },
            { ConsoleColor.DarkRed, "[ERROR] " },
            { ConsoleColor.Cyan, "[NOTIFY] " },
            { ConsoleColor.Blue, "[INFO] " },
            { ConsoleColor.DarkBlue, "[INFO] " }
        };

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.Green)
        {
            string prefix = GetPrefix(color);
            string date = $"[{DateTime.Now:HH:mm:ss}] ";

            // Assemble the full log line (prefix + date + message)
            string finalMessage = $"{prefix}{date}{message}";

            // Write the full line atomically with one Console.WriteLine (avoids color mismatches)
            Console.ForegroundColor = color;
            Console.WriteLine(finalMessage);
            Console.ResetColor();

            // Write to log file
            WriteToLog(finalMessage);
        }

        private static void WriteToLog(string message)
        {
            if (!Main.Settings.DisableLogWriting)
                return;

            try
            {
                using var fileStream = new FileStream(Main.Config.LogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using var writer = new StreamWriter(fileStream);
                    writer.WriteLine(message);
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
