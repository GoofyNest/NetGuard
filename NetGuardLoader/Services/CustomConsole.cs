namespace NetGuardLoader.Services
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
            Console.ForegroundColor = color;
            Console.Write(prefix);
            Console.ResetColor();

            string date = $"[{DateTime.Now:HH:mm:ss}] ";

            // Display timestamp
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(date);
            Console.ResetColor();

            // Display message
            Console.WriteLine(message);
        }

        private static string GetPrefix(ConsoleColor color)
        {
            return ColorPrefixes.TryGetValue(color, out string? prefix)
                ? prefix
                : "[NORMAL] ";
        }
    }
}