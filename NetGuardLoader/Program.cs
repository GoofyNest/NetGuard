using System.Runtime.Loader;
using NetGuardLoader.Classes;
using NetGuardLoader.Services;
using Newtonsoft.Json;

namespace NetGuardLoader
{
    internal class Program
    {
        public static Config Config = new();

        public static string module = "";
        public static int moduleIndex = -1;

        private static void LoadSettings()
        {
            if (File.Exists(Config.BindingsPath))
            {
                Custom.WriteLine($"Loaded existing settings from {Config.BindingsPath}");

                var bindingsContent = File.ReadAllText(Config.BindingsPath);

                var tempConfig = JsonConvert.DeserializeObject<Config>(bindingsContent);

                if (tempConfig != null)
                {
                    Config = tempConfig;
                }

                foreach (var Module in Config.ModuleBinding)
                {
                    string moduleName = Module.ModuleType.ToString();

                    if (Module.Name.Contains("Example"))
                    {
                        Custom.WriteLine($"Seems like you still have {moduleName} named Example", ConsoleColor.Cyan);
                    }
                }

                File.WriteAllText(Config.BindingsPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
            }
            else
            {
                Custom.WriteLine($"Creating new settings file {Config.BindingsPath}");

                Config.ModuleBinding.Add(new ModuleSettings() { Name = "Example #1", ModuleType = ModuleType.GatewayModule });

                for (var i = 0; i < 3; i++)
                {
                    Config.ModuleBinding.Add(new ModuleSettings() 
                    { 
                        Name = "Example #" + (i + 1), 
                        GuardPort = 15884 + i, 
                        ModulePort = 5884 + i, 
                        ModuleType = ModuleType.AgentModule 
                    });
                }

                File.WriteAllText(Config.BindingsPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
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
                    bool v = int.TryParse(args[0], out moduleIndex);
                }
                catch (Exception ex)
                {
                    Custom.WriteLine(ex.ToString(), ConsoleColor.Red);
                }

                try
                {
                    var Module = Config.ModuleBinding[moduleIndex];

                    byte[] dllBytes = File.ReadAllBytes(Config.DllPath);
                    var _response = Convert.ToBase64String(dllBytes);

                    Custom.WriteLine($"Loading dll {Config.DllPath}", ConsoleColor.Magenta);

                    // Load DLL from byte array
                    using var stream = new MemoryStream(dllBytes);
                    var assemblyLoadContext = new AssemblyLoadContext("TestModuleContext", isCollectible: true);
                    var loadedAssembly = assemblyLoadContext.LoadFromStream(stream);

                    // Set arguments for the loaded module
                    object[] sharedArgs = [moduleIndex];

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

            Console.ForegroundColor = ConsoleColor.Yellow;
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
