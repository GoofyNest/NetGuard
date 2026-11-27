using Module.Config;
using Module.Helpers;
using Module.Helpers.ClientData;
using Module.Networking;
using Module.PacketHandler.Agent.Client;
using Module.PacketHandler.Filtering;
using Module.Services;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Module
{
    public class Main
    {
        public static NetworkConfig Config { get; set; } = new();
        public static ModuleSettings Module { get; set; } = new();
        private static AppSettings _settings = new();
        public static AppSettings Settings
        {
            get => Volatile.Read(ref _settings);
            set => Volatile.Write(ref _settings, value ?? throw new ArgumentNullException(nameof(value)));
        }
        private static ClientDataSnapshot _data = new();
        public static ClientDataSnapshot Data => Volatile.Read(ref _data);
        public static ConcurrentDictionary<int, ItemData> Items => Data.Items;
        public static ConcurrentDictionary<int, SkillData> Skills => Data.Skills;
        public static ConcurrentDictionary<int, CharData> Chars => Data.Chars;
        internal static void PublishData(ClientDataSnapshot data) => Volatile.Write(ref _data, data);

        private static SettingsReloader? _settingsWatcher;
        private static AsyncServer? _server;
        private static LiveNetworkStats? _liveStats;

        static void ConsolePoolThread()
        {
            while (true)
            {
                string? cmd = Console.ReadLine();
                if (cmd == null)
                {
                    if (DesktopDiagnostics.Current != null) Environment.Exit(0);
                    return;
                }
                if (cmd.StartsWith(NetGuard.DesktopProtocol.Protocol.Prefix, StringComparison.Ordinal))
                {
                    DesktopDiagnostics.Current?.HandleCommandAsync(cmd[NetGuard.DesktopProtocol.Protocol.Prefix.Length..]).GetAwaiter().GetResult();
                    continue;
                }

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
                            $"[console] live={FilterRegistry.LiveSessions.Count} {NetworkMetrics.Snapshot()}",
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
            if (!isMainFunction) { _settingsWatcher?.ReloadNow(); return; }

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

            // Existing configuration must be complete and valid before publication.
            if (File.Exists(Config.SettingsPath))
                Settings = SettingsFile.Parse(File.ReadAllText(Config.SettingsPath));
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(Config.SettingsPath))!);
                File.WriteAllText(Config.SettingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
        }

        private static void WatchSettingsFile()
        {
            _settingsWatcher?.Dispose();
            _settingsWatcher = new SettingsReloader(Config.SettingsPath,
                settings => { Settings = settings; Custom.WriteLine("Settings reloaded.", ConsoleColor.Cyan); },
                ex => Custom.WriteLine($"Settings retained; reload failed: {ex.Message}", ConsoleColor.Red));
        }
        public static void StartProgram(int moduleIndex)
        {
            // 1. Shutdown handler — flush logs on Ctrl+C / window close
            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                try { _settingsWatcher?.Dispose(); } catch { }
                try { Reader.Shutdown(); } catch { }
                try
                {
                    if (_server != null && !_server.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(5)))
                        Custom.WriteLine("[shutdown] connection cleanup exceeded exit deadline", ConsoleColor.Yellow);
                }
                catch { }
                try { _liveStats?.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(2)); } catch { }
                try { SessionBus.Stop(); } catch { }
                try { Custom.WriteLine("[shutdown] exiting", ConsoleColor.Yellow); } catch { }
                try { Custom.FlushAndStop(); } catch { }
            };

            if (Environment.GetEnvironmentVariable("NETGUARD_DESKTOP") == "1")
                DesktopDiagnostics.Current = new DesktopDiagnostics(moduleIndex, Console.Out);

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

            if (DesktopDiagnostics.Current == null) Console.Title = $"NetGuard | {Module.Name}";

            Config.LogFile = Path.Combine(Config.LogFolder, Module.Name + ".txt");
            Directory.CreateDirectory(Config.LogFolder);

            if (File.Exists(Config.LogFile))
                File.Delete(Config.LogFile);

            // 4. Start bus
            SessionBus.ApiPort = Module.ApiPort;
            SessionBus.ApiPassword = Module.ApiPassword;
            SessionBus.ListenAddress = Module.ApiListenIP;
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

            string liveStatsPath = Path.Combine(Config.LogFolder, $"network-{moduleIndex}.stats.txt");
            _liveStats = new LiveNetworkStats(liveStatsPath);
            Custom.WriteLine($"[stats] Live snapshot every 10 seconds: {Path.GetFullPath(liveStatsPath)}", ConsoleColor.Cyan);

            // 8. Console command thread (background, so it doesn't block shutdown)
            new Thread(ConsolePoolThread) { IsBackground = true, Name = "console" }.Start();
        }

        static async Task StartAsyncServer()
        {
            _server = new AsyncServer();
            await _server.StartAsync(Module.GuardIP, Module.GuardPort, Module.ModuleType);
        }
    }
}
