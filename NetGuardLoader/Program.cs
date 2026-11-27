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
        private static bool _loaded;

        private static void LoadSettings()
        {
            string name = "Local\\NetGuard.Config." + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(Path.GetFullPath(Config.BindingsPath))))[..16];
            using var guard = new Mutex(false, name);
            bool entered;
            try { entered = guard.WaitOne(TimeSpan.FromSeconds(10)); } catch (AbandonedMutexException) { entered = true; }
            if (!entered) throw new IOException("Another module is updating network configuration.");
            try { LoadSettingsCore(); } finally { guard.ReleaseMutex(); }
        }

        private static void LoadSettingsCore()
        {
            if (File.Exists(Config.BindingsPath))
            {
                Custom.WriteLine($"Loaded existing settings from {Config.BindingsPath}");

                var bindingsContent = File.ReadAllText(Config.BindingsPath);

                var tempConfig = JsonConvert.DeserializeObject<NetworkConfig>(bindingsContent);

                if (tempConfig != null)
                {
                    Config = tempConfig;

                    // Generate the group key ONCE if any module is missing one.
                    var needsKey = Config.ModuleBinding.Any(m => string.IsNullOrEmpty(m.ApiPassword));
                    if (needsKey)
                    {
                        foreach (var m in Config.ModuleBinding)
                        {
                            var Key = Convert.ToHexString(
                            System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

                            if (string.IsNullOrEmpty(m.ApiPassword))
                                m.ApiPassword = Key;
                        }

                        Custom.WriteLine("Generated shared API key for existing config.", ConsoleColor.Cyan);
                    }
                }

                // Backfill ApiPort if missing.
                for (var i = 0; i < Config.ModuleBinding.Count; i++)
                {
                    if (Config.ModuleBinding[i].ApiPort == 0)
                        Config.ModuleBinding[i].ApiPort = 21000 + i;
                }

                foreach (var Module in Config.ModuleBinding)
                {
                    string moduleName = Module.ModuleType.ToString();

                    if (Module.Name.Contains("Example"))
                    {
                        Custom.WriteLine($"Seems like you still have {moduleName} named Example", ConsoleColor.Cyan);
                    }
                }

                // Preserve settings owned by Module which are not represented in the loader DTO.
                var document = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(Config.BindingsPath));
                bool changed = false;
                if (document["ModuleBinding"] is Newtonsoft.Json.Linq.JArray bindings)
                    for (int i = 0; i < Math.Min(bindings.Count, Config.ModuleBinding.Count); i++)
                    {
                        changed |= (int?)bindings[i]["ApiPort"] != Config.ModuleBinding[i].ApiPort
                            || (string?)bindings[i]["ApiPassword"] != Config.ModuleBinding[i].ApiPassword;
                        bindings[i]["ApiPort"] = Config.ModuleBinding[i].ApiPort;
                        bindings[i]["ApiPassword"] = Config.ModuleBinding[i].ApiPassword;
                    }
                if (changed) File.WriteAllText(Config.BindingsPath, document.ToString(Formatting.Indented));
            }
            else
            {
                Custom.WriteLine($"Creating new settings file {Config.BindingsPath}");

                Config.ModuleBinding.Add(new ModuleSettings() { Name = "Example #1", ModuleType = ModuleType.GatewayModule });

                for (var i = 0; i < 3; i++)
                {
                    var Key = Convert.ToHexString(
                        System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

                    Config.ModuleBinding.Add(new ModuleSettings()
                    {
                        Name = "Example #" + (i + 1),
                        GuardPort = 15884 + i,
                        ModulePort = 5884 + i,
                        ModuleType = ModuleType.AgentModule,
                        ApiPort = 21000 + i,
                        ApiPassword = Key
                    });
                }

                File.WriteAllText(Config.BindingsPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
            }
        }

        private static void LoadDll(string[] args)
        {
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
                method.Invoke(instance, [moduleIndex]);
                _loaded = true;

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
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            if (args.Length == 0)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Path.Combine(AppContext.BaseDirectory, "NetGuard.exe")) { UseShellExecute = true });
                return;
            }
            bool desktop = args[0] == "--desktop";
            if (desktop)
            {
                args = args.Skip(1).ToArray();
                Environment.SetEnvironmentVariable("NETGUARD_DESKTOP", "1");
                Console.InputEncoding = System.Text.Encoding.UTF8;
                Console.OutputEncoding = new System.Text.UTF8Encoding(false);
            }
            if (args.Length != 1) { Environment.ExitCode = 1; return; }
            if (!desktop)
            {
            ConsoleHelper.EnableQuickEdit();

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

            }
            Directory.CreateDirectory("config");

            LoadSettings();

            // In Program.Main, after LoadDll(args):
            LoadDll(args);
            if (!_loaded) { Environment.ExitCode = 1; return; }

            // Block Main forever. The process exits when someone calls Environment.Exit
            // or Ctrl+C is pressed.
            new ManualResetEventSlim(false).Wait();
        }
    }
}
