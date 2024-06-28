using System;

namespace NetGuard.Services
{
    public static class Custom
    {
        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            var prefix = "";
            switch (color)
            {
                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    prefix = "[WARN] ";
                    break;

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    prefix = "[DEBUG] ";
                    break;

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    prefix = "[ERROR] ";
                    break;

                case ConsoleColor.Cyan:
                    prefix = "[NOTIFY] ";
                    break;

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    prefix = "[INFO] ";
                    break;

            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                prefix = "[NORMAL] ";
                color = ConsoleColor.Green;
            }

            Console.ForegroundColor = color;
            Console.Write(prefix);
            Console.ResetColor();

            var splitMessage = message.Split(' ');

            var newMessage = "";
            for(var i = 0; i< splitMessage.Length; i++)
            {
                var tempMsg = splitMessage[i];

                if (tempMsg == "[C->S]")
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(tempMsg+" ");
                    Console.ResetColor();
                    continue;
                }
                else if(tempMsg == "[S->C]")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(tempMsg+" ");
                    Console.ResetColor();
                    continue;
                }
                newMessage += tempMsg + " ";
            }

            newMessage = newMessage.Trim();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            Console.ResetColor();

            Console.WriteLine($"{newMessage}");

        }
    }
}
