using NetGuardLoader.Classes;
using NetGuardLoader.Functions;
using NetGuardLoader.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetGuardLoader
{
    public interface IPlugin
    {
        int Execute(object[] args);
    }

    internal class Program
    {
        public static string discordId = "";
        public static string discordName = "";
        public static string date = "";

        public static AppDomain _appDomain = AppDomain.CurrentDomain;
        public static Assembly _assembly = null;

        public static string _sig = Hash.read(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        public static Config _config = new Config();

        public static string module = "";
        public static int moduleIndex = -1;

        static void ConsolePoolThread()
        {
            while (true)
            {
                Thread.Sleep(1);

                string cmd = Console.ReadLine();
            }
        }

        static void Main(string[] args)
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

            var path = Path.Combine("config");

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var settingsPath = Path.Combine(path, "settings.json");

            if (!File.Exists(settingsPath))
            {
                Custom.WriteLine($"No settings found, creating new example configuration", ConsoleColor.Cyan);

                _config._gatewayModules.Add(new GatewaySettings() { name = "Example #1" });

                for (var i = 0; i<3; i++)
                    _config._agentModules.Add(new AgentSettings() { name = "Example #" + i });

                File.WriteAllText(settingsPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
            }
            else
            {
                Custom.WriteLine($"Loaded existing settings from {settingsPath}");

                _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(settingsPath));

                foreach(var _module in _config._agentModules)
                {
                    if(_module.name.Contains("Example"))
                    {
                        Custom.WriteLine($"Seems like you still have agentModules named Example", ConsoleColor.Cyan);
                        break;
                    }
                }

                foreach (var _module in _config._gatewayModules)
                {
                    if (_module.name.Contains("Example"))
                    {
                        Custom.WriteLine($"Seems like you still have gatewayModules named Example", ConsoleColor.Cyan);
                        break;
                    }
                }
            }

            if (args.Length <= 0)
            {
                Custom.WriteLine("You need to start the loader with arguments", ConsoleColor.Cyan);
                Custom.WriteLine("Example below:", ConsoleColor.Cyan);
                Custom.WriteLine("start NetGuard.exe <gateway/agent> <moduleIndex>", ConsoleColor.Cyan);
                Custom.WriteLine("start NetGuard.exe gateway 0", ConsoleColor.Cyan);
                Custom.WriteLine("start NetGuard.exe agent 0", ConsoleColor.Cyan);
                Custom.WriteLine("start NetGuard.exe agent 1", ConsoleColor.Cyan);

                if(!File.Exists("example_gateway.bat"))
                    File.WriteAllText("example_gateway.bat", "echo off\nstart NetGuard.exe gateway 0");

                if(!File.Exists("example_agent.bat"))
                    File.WriteAllText("example_agent.bat", "echo off\nstart NetGuard.exe agent 0");
            }
            else
            {
                module = args[0];

                if (args.Count() == 1)
                    moduleIndex = 0;
                else
                    moduleIndex = int.Parse(args[1]);

                switch(module)
                {
                    case "gateway":
                        {
                            try
                            {
                                var _module = _config._gatewayModules[moduleIndex];
                                
                                byte[] dllBytes = File.ReadAllBytes("C:\\Users\\computer\\source\\repos\\NetGuard\\GatewayModule\\bin\\Release\\GatewayModule.dll");
                                var _reponse = Convert.ToBase64String(dllBytes);
                                
                                _appDomain.SetData("Assembly", dllBytes);
                                _appDomain.DoCallBack(Callback.init);
                            }
                            catch(Exception ex) {
                                Custom.WriteLine($"Could not load the gatewayModule", ConsoleColor.Red);
                                Custom.WriteLine(ex.Message, ConsoleColor.Red);
                            }
                        }
                        break;

                    case "agent":
                        {
                            try
                            {
                                var _module = _config._agentModules[moduleIndex];

                                byte[] dllBytes = File.ReadAllBytes("C:\\Users\\computer\\source\\repos\\NetGuard\\AgentModule\\bin\\Release\\AgentModule.dll");
                                var _reponse = Convert.ToBase64String(dllBytes);
                                
                                _appDomain.SetData("Assembly", dllBytes);
                                _appDomain.DoCallBack(Callback.init);
                            }
                            catch (Exception ex)
                            {
                                Custom.WriteLine($"Could not load the agentModule", ConsoleColor.Red);
                                Custom.WriteLine(ex.Message, ConsoleColor.Red);
                            }
                        }
                        break;

                    default:
                        Custom.WriteLine($"Trying to load an unsupported module?", ConsoleColor.Red);
                        break;
                }
            }

            //Custom.WriteLine($"Loading settings");





            //byte[] dllBytes = File.ReadAllBytes("C:\\Users\\computer\\source\\repos\\NetGuard\\GatewayContext\\bin\\Debug\\GatewayContext.dll");
            //var _reponse = Convert.ToBase64String(dllBytes);
            //
            //_appDomain.SetData("Assembly", dllBytes);
            //_appDomain.DoCallBack(Callback.init);

            //new Thread(ConsolePoolThread).Start();
        }
    }
}
