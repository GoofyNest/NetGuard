using Module.Config;
using Module.Helpers;
using Module.Helpers.ClientData;
using Module.Networking;
using Module.PacketHandler.Agent.Client;
using Module.PacketHandler.Filtering;
using Module.Services;
using Newtonsoft.Json;

namespace Module
{
    public class Main
    {
        public static NetworkConfig Config { get; set; } = new();
        public static ModuleSettings Module { get; set; } = new();
        public static AppSettings Settings { get; set; } = new();
        public static Dictionary<int, ItemData> Items { get; set; } = [];
        public static Dictionary<int, SkillData> Skills { get; set; } = [];

        private static FileSystemWatcher? _settingsWatcher;

        static void ConsolePoolThread()
        {
            while (true)
            {
                string? cmd = Console.ReadLine();
                if (cmd == null) return;

                switch (cmd.Trim().ToLowerInvariant())
                {
                    case "test":
                        Custom.WriteLine("[console] test command received", ConsoleColor.Cyan);
                        break;
                    case "gc":
                        GC.Collect();
                        Custom.WriteLine("[console] forced GC", ConsoleColor.Cyan);
                        break;
                    case "stats":
                        Custom.WriteLine(
                            $"[console] live={FilterRegistry.LiveSessions.Count}",
                            ConsoleColor.Cyan);
                        break;
                    case "exit":
                    case "quit":
                        Environment.Exit(0);
                        break;
                    case "":
                        break;
                    default:
                        Custom.WriteLine($"[console] unknown: {cmd}", ConsoleColor.Yellow);
                        break;
                }
            }
        }

        private static void LoadSettings(bool isMainFunction)
        {
            if (!isMainFunction)
            {
                Custom.WriteLine("changes detected", ConsoleColor.Yellow);
                if (File.Exists(Config.SettingsPath))
                {
                    try
                    {
                        using var stream = new FileStream(Config.SettingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var reader = new StreamReader(stream);
                        var settingsContent = reader.ReadToEnd();

                        var tempSettings = JsonConvert.DeserializeObject<AppSettings>(settingsContent);
                        if (tempSettings != null)
                            Settings = tempSettings;
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"Error reading settings file: {ex.Message}", ConsoleColor.Red);
                    }
                }
                return;
            }

            // Load NetworkConfig
            if (File.Exists(Config.BindingsPath))
            {
                try
                {
                    using var stream = new FileStream(Config.BindingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(stream);
                    var bindingsContent = reader.ReadToEnd();

                    var tempConfig = JsonConvert.DeserializeObject<NetworkConfig>(bindingsContent);
                    if (tempConfig != null)
                        Config = tempConfig;
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"Error reading bindings file: {ex.Message}", ConsoleColor.Red);
                }
            }

            // Load AppSettings
            if (File.Exists(Config.SettingsPath))
            {
                try
                {
                    using var stream = new FileStream(Config.SettingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(stream);
                    var settingsContent = reader.ReadToEnd();

                    var tempSettings = JsonConvert.DeserializeObject<AppSettings>(settingsContent);
                    if (tempSettings != null)
                        Settings = tempSettings;

                    Directory.CreateDirectory(Path.GetDirectoryName(Config.SettingsPath)!);
                    File.WriteAllText(Config.SettingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"Error reading settings file: {ex.Message}", ConsoleColor.Red);
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Config.SettingsPath)!);
                File.WriteAllText(Config.SettingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
        }

        private static void WatchSettingsFile()
        {
            Custom.WriteLine("Starting WatchSettingsFile", ConsoleColor.Yellow);

            _settingsWatcher = new()
            {
                Path = Path.GetDirectoryName(Config.SettingsPath)!,
                Filter = Path.GetFileName(Config.SettingsPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                EnableRaisingEvents = true
            };

            _settingsWatcher.Changed += (sender, e) => LoadSettings(false);

            _settingsWatcher.Error += (sender, e) =>
            {
                Custom.WriteLine($"FileSystemWatcher error: {e.GetException()?.Message}", ConsoleColor.Red);
            };
        }

        public static void StartProgram(int moduleIndex)
        {
            // 1. Shutdown handler — flush logs on Ctrl+C / window close
            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                try { Custom.WriteLine("[shutdown] exiting", ConsoleColor.Yellow); } catch { }
                try { Custom.FlushAndStop(); } catch { }
            };

            // 2. Start the logger
            Custom.StartDrain();

            // 3. Thread pool tuning
            ThreadPool.GetMinThreads(out var wMin, out var ioMin);
            ThreadPool.SetMinThreads(
                Math.Max(wMin, Environment.ProcessorCount * 4),
                Math.Max(ioMin, Environment.ProcessorCount * 4));

            // 3. Load configuration
            LoadSettings(true);

            Module = Config.ModuleBinding[moduleIndex];

            Console.Title = $"NetGuard | {Module.Name}";

            Config.LogFile = Path.Combine(Config.LogFolder, Module.Name + ".txt");
            Directory.CreateDirectory(Config.LogFolder);

            if (File.Exists(Config.LogFile))
                File.Delete(Config.LogFile);

            // 4. Start bus
            SessionBus.ApiPort = Module.ApiPort;
            SessionBus.ApiPassword = Module.ApiPassword;
            SessionBus.StartApiListener();

            // 5. Start filter loader
            var filterLoader = new FilterLoader(Config.FiltersPath);
            filterLoader.Start();
            FilterRegistry.Loader = filterLoader;

            // 6. Agent-specific init
            if (Module.ModuleType == ModuleType.AgentModule)
            {
                Reader.Init();

                if (Settings.Agent.GameMasterConfig.GMs.Count == 0)
                {
                    var example = new AppSettings.Operators
                    {
                        Username = "netguard_example_325125124123124",
                        NoPinkChat = false,
                        ShouldSpawnVisible = false
                    };
                    example.Permissions = example.DefaultPermissions;

                    Settings.Agent.GameMasterConfig.GMs.Add(example);

                    File.WriteAllText(Config.SettingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
                }
            }

            WatchSettingsFile();

            Custom.WriteLine(
                $"Guard IP: {Module.GuardIP}, Guard Port: {Module.GuardPort}, " +
                $"Module IP: {Module.ModuleIP}, Module Port: {Module.ModulePort}",
                ConsoleColor.Magenta);

            // 7. Start the async server with error handling
            Task.Run(async () =>
            {
                try { await StartAsyncServer(); }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[fatal] Server startup failed: {ex}", ConsoleColor.Red);
                    Environment.Exit(1);
                }
            });

            // 8. Console command thread (background, so it doesn't block shutdown)
            new Thread(ConsolePoolThread) { IsBackground = true, Name = "console" }.Start();
        }

        static async Task StartAsyncServer()
        {
            AsyncServer server = new();
            await server.StartAsync(Module.GuardIP, Module.GuardPort, Module.ModuleType);
        }
    }
}