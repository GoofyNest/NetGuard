using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard.Services
{
    class ConsoleHelper
    {
        private const uint ENABLE_QUICK_EDIT = 0x0040;
        private const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public static void DisableQuickEdit()
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // Get current console mode
            if (!GetConsoleMode(consoleHandle, out uint mode))
            {
                return;
            }

            // Remove Quick Edit Mode
            mode &= ~ENABLE_QUICK_EDIT;

            // Set the new mode
            SetConsoleMode(consoleHandle, mode);
        }
    }
}
