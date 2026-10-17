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

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.Green)
        {
            string prefix = GetPrefix(color);
            string date = $"[{DateTime.Now:HH:mm:ss}] ";
            string finalMessage = $"{prefix}{date}{message}";

            Console.ForegroundColor = color;
            Console.WriteLine(finalMessage);
            Console.ResetColor();

            WriteToLog(finalMessage);
        }

        private static void WriteToLog(string message)
        {
            var path = Main.Config.LogFile;
            if (string.IsNullOrEmpty(path)) return;
            if (Main.Settings.DisableLogWriting) return;

            try
            {
                using var fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using var writer = new StreamWriter(fileStream);
                writer.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CustomConsole] {ex.GetType().Name}: {ex.Message}");
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