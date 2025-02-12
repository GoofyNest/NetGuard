using System.Runtime.Loader;
using NetGuardLoader.Classes;
using NetGuardLoader.Services;
using Newtonsoft.Json;

namespace NetGuardLoader
{
    internal class Program
    {
        public static Config _config = new();
        public static Settings _settings = new();

        public static string module = "";
        public static int moduleIndex = -1;

        private static void LoadSettings()
        {
            if (File.Exists(_config.bindingsPath))
            {
                Custom.WriteLine($"Loaded existing settings from {_config.bindingsPath}");

                var bindingsContent = File.ReadAllText(_config.bindingsPath);

                var tempConfig = JsonConvert.DeserializeObject<Config>(bindingsContent);

                if (tempConfig != null)
                {
                    _config = tempConfig;
                }

                foreach (var _module in _config.ModuleBinding)
                {
                    string moduleName = _module.moduleType.ToString();

                    if (_module.name.Contains("Example"))
                    {
                        Custom.WriteLine($"Seems like you still have {moduleName} named Example", ConsoleColor.Cyan);
                    }
                }

                File.WriteAllText(_config.bindingsPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
            }
            else
            {
                Custom.WriteLine($"Creating new settings file {_config.bindingsPath}");

                _config.ModuleBinding.Add(new ModuleSettings() { name = "Example #1", moduleType = ModuleType.GatewayModule });

                for (var i = 0; i < 3; i++)
                {
                    _config.ModuleBinding.Add(new ModuleSettings() { name = "Example #" + (i + 1), guardPort = 15884 + i, modulePort = 5884 + i, moduleType = ModuleType.AgentModule });
                }

                File.WriteAllText(_config.bindingsPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
            }

            if (File.Exists(_config.settingsPath))
            {
                var settingsContent = File.ReadAllText(_config.settingsPath);

                var tempSettings = JsonConvert.DeserializeObject<Settings>(settingsContent);
                if (tempSettings != null)
                {
                    _settings = tempSettings;
                }

                File.WriteAllText(_config.settingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
            }
            else
            {
                File.WriteAllText(_config.settingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
            }
        }

        private static void LoadDll(string[] args)
        {
            if (args.Length <= 0)
            {
                Custom.WriteLine("You need to start the loader with arguments", ConsoleColor.Cyan);
                Custom.WriteLine("Example below:", ConsoleColor.Cyan);
                Custom.WriteLine("start NetGuardLoader.exe <moduleIndex>", ConsoleColor.Cyan);
                Custom.WriteLine("start NetGuardLoader.exe 0", ConsoleColor.Cyan);
                Custom.WriteLine("start NetGuardLoader.exe 1", ConsoleColor.Cyan);
                Custom.WriteLine("start NetGuardLoader.exe 2", ConsoleColor.Cyan);

                if (!File.Exists("example_gateway.bat"))
                    File.WriteAllText("example_gateway.bat", "@echo on\ncmd /k \"NetGuardLoader.exe 0\"\npause");

                if (!File.Exists("example_agent.bat"))
                    File.WriteAllText("example_agent.bat", "@echo on\ncmd /k \"NetGuardLoader.exe 1\"\npause");
            }
            else
            {
                module = args[0];
                moduleIndex = 0;

                try
                {
                    int.TryParse(args[0], out moduleIndex);
                }
                catch (Exception ex)
                {
                    Custom.WriteLine(ex.ToString(), ConsoleColor.Red);
                }

                try
                {
                    var _module = _config.ModuleBinding[moduleIndex];

                    byte[] dllBytes = File.ReadAllBytes(_config.dllPath);
                    var _response = Convert.ToBase64String(dllBytes);

                    Custom.WriteLine($"Loading dll {_config.dllPath}", ConsoleColor.DarkMagenta);

                    // Load DLL from byte array
                    using var stream = new MemoryStream(dllBytes);
                    var assemblyLoadContext = new AssemblyLoadContext("TestModuleContext", isCollectible: true);
                    var loadedAssembly = assemblyLoadContext.LoadFromStream(stream);

                    // Set arguments for the loaded module
                    object[] sharedArgs = { moduleIndex };

                    // Get type and method from the loaded assembly
                    var targetType = loadedAssembly.GetType("Module.Main");
                    var method = targetType?.GetMethod("StartProgram");

                    if (targetType != null && method != null)
                    {
                        // Create an instance of Module.Main
                        var instance = Activator.CreateInstance(targetType);

                        // Invoke StartProgram on the instance
                        method.Invoke(instance, sharedArgs);
                    }
                    else
                    {
                        Custom.WriteLine("Main class or StartProgram method not found.", ConsoleColor.Red);
                    }

                    assemblyLoadContext.Unload();
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"Could not load the agentModule", ConsoleColor.Red);
                    Custom.WriteLine(ex.ToString(), ConsoleColor.Red);
                }
            }
        }

        static void Main(string[] args)
        {
            ConsoleHelper.DisableQuickEdit();

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
                                    Created by                               gooofie");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Directory.CreateDirectory("config");

            LoadSettings();

            LoadDll(args);
        }
    }
}
