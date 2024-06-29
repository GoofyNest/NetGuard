using System;
using System.Collections.Generic;

namespace NetGuard.Services
{
    public static class Custom
    {
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

            // Display timestamp
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            Console.ResetColor();

            // Display message
            Console.WriteLine(message);
        }

        private static string GetPrefix(ConsoleColor color)
        {
            if (ColorPrefixes.TryGetValue(color, out string prefix))
            {
                return prefix;
            }

            // Default prefix and color if not found
            return "[NORMAL] ";
        }
    }
}
