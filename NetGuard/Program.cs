using NetGuard.Services;
using NetGuard.Functions;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace NetGuard
{
    internal class Program
    {
        public static string discordId = "";
        public static string discordName = "";
        public static string date = "";

        public static AppDomain _appDomain = AppDomain.CurrentDomain;
        public static Assembly _assembly = null;

        public static string _sig = Hash.read(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        static void ConsolePoolThread()
        {
            while (true)
            {
                Thread.Sleep(1);

                string cmd = Console.ReadLine();
            }
        }

        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Title = "NetGuard | Loader";
            Console.WriteLine(@"
                                                                                                                     
  _____   ______        ______   _________________      _____     ____   ____        ____        _____        _____   
 |\    \ |\     \   ___|\     \ /                 \ ___|\    \   |    | |    |  ____|\   \   ___|\    \   ___|\    \  
  \\    \| \     \ |     \     \\______     ______//    /\    \  |    | |    | /    /\    \ |    |\    \ |    |\    \ 
   \|    \  \     ||     ,_____/|  \( /    /  )/  |    |  |____| |    | |    ||    |  |    ||    | |    ||    | |    |
    |     \  |    ||     \--'\_|/   ' |   |   '   |    |    ____ |    | |    ||    |__|    ||    |/____/ |    | |    |
    |      \ |    ||     /___/|       |   |       |    |   |    ||    | |    ||    .--.    ||    |\    \ |    | |    |
    |    |\ \|    ||     \____|\     /   //       |    |   |_,  ||    | |    ||    |  |    ||    | |    ||    | |    |
    |____||\_____/||____ '     /|   /___//        |\ ___\___/  /||\___\_|____||____|  |____||____| |____||____|/____/|
    |    |/ \|   |||    /_____/ |  |`   |         | |   /____ / || |    |    ||    |  |    ||    | |    ||    /    | |
    |____|   |___|/|____|     | /  |____|          \|___|    | /  \|____|____||____|  |____||____| |____||____|____|/ 
      \(       )/    \( |_____|/     \(              \( |____|/      \(   )/    \(      )/    \(     )/    \(    )/   
       '       '      '    )/         '               '   )/          '   '      '      '      '     '      '    '    
                           '                              '                                                           
                                    Created by                               Réna");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            byte[] dllBytes = File.ReadAllBytes("C:\\Users\\computer\\source\\repos\\NetGuard\\GatewayContext\\bin\\Debug\\GatewayContext.dll");
            var _reponse = Convert.ToBase64String(dllBytes);

            _appDomain.SetData("Assembly", dllBytes);
            _appDomain.DoCallBack(Callback.init);

            new Thread(ConsolePoolThread).Start();
        }
    }
}
