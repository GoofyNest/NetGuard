using System.Runtime.Loader;
using NetGuardLoader.Config;
using NetGuardLoader.Services;
using Newtonsoft.Json;

namespace NetGuardLoader
{
    internal class Program
    {
        public static NetworkConfig Config = new();

        public static string module = "";
        public static int moduleIndex = -1;

        private static void LoadSettings()
        {
            if (File.Exists(Config.BindingsPath))
            {
                Custom.WriteLine($"Loaded existing settings from {Config.BindingsPath}");

                var bindingsContent = File.ReadAllText(Config.BindingsPath);

                var tempConfig = JsonConvert.DeserializeObject<NetworkConfig>(bindingsContent);

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

                return;
            }

            module = args[0];

            if (!int.TryParse(args[0], out moduleIndex))
            {
                Custom.WriteLine($"Invalid module index: {args[0]}", ConsoleColor.Red);
                return;
            }

            if (moduleIndex < 0 || moduleIndex >= Config.ModuleBinding.Count)
            {
                Custom.WriteLine($"Module index {moduleIndex} out of range (0..{Config.ModuleBinding.Count - 1})", ConsoleColor.Red);
                return;
            }

            var moduleSettings = Config.ModuleBinding[moduleIndex];

            try
            {
                Custom.WriteLine($"Loading dll {Config.DllPath} (index {moduleIndex}, module {moduleSettings.ModuleType})", ConsoleColor.Magenta);

                var dllPath = Path.GetFullPath(Config.DllPath);
                var dllDir = Path.GetDirectoryName(dllPath) ?? AppContext.BaseDirectory;

                var alc = new AssemblyLoadContext("NetGuard.Module", isCollectible: false);

                // Resolve any dependency the module needs, looking next to Module.dll,
                // then next to the loader exe, then reusing an already-loaded assembly.
                alc.Resolving += (context, name) =>
                {
                    var fileName = name.Name + ".dll";

                    // 1. Next to Module.dll
                    var candidate = Path.Combine(dllDir, fileName);
                    if (File.Exists(candidate))
                    {
                        try { return context.LoadFromAssemblyPath(candidate); }
                        catch { /* fall through */ }
                    }

                    // 2. Next to the loader exe
                    candidate = Path.Combine(AppContext.BaseDirectory, fileName);
                    if (File.Exists(candidate))
                    {
                        try { return context.LoadFromAssemblyPath(candidate); }
                        catch { /* fall through */ }
                    }

                    // 3. Already loaded in the default context? Reuse it.
                    var alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.GetName().Name == name.Name);

                    return alreadyLoaded;
                };

                var loadedAssembly = alc.LoadFromAssemblyPath(dllPath);

                var targetType = loadedAssembly.GetType("Module.Main");
                var method = targetType?.GetMethod("StartProgram");

                if (targetType == null || method == null)
                {
                    Custom.WriteLine("Main class or StartProgram method not found.", ConsoleColor.Red);
                    return;
                }

                var instance = Activator.CreateInstance(targetType);
                method.Invoke(instance, new object[] { moduleIndex });

                // Do NOT call alc.Unload() — the module runs background threads and
                // keeps running after StartProgram returns. Keeping the ALC alive for
                // the process lifetime is correct here.
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Could not load the {moduleSettings.ModuleType}", ConsoleColor.Red);
                Custom.WriteLine(ex.ToString(), ConsoleColor.Red);
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