using Module.Config;
using Module.Networking;
using Module.Helpers;
using Module.Helpers.ClientData;
using Module.Services;
using Newtonsoft.Json;
using Module.PacketHandler.Agent.Client;

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
                Thread.Sleep(1);

                string cmd = Console.ReadLine() ?? string.Empty;

                if(cmd == "test")
                {

                }
            }
        }

        private static void LoadSettings(bool isMainFunction)
        {
            if (isMainFunction == false)
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
                        {
                            Settings = tempSettings;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading settings file: {ex.Message}");
                    }
                }
                return;
            }

            if (File.Exists(Config.BindingsPath))
            {
                try
                {
                    using var stream = new FileStream(Config.BindingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(stream);
                    var bindingsContent = reader.ReadToEnd();

                    var tempConfig = JsonConvert.DeserializeObject<NetworkConfig>(bindingsContent);

                    if (tempConfig != null)
                    {
                        Config = tempConfig;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading settings file: {ex.Message}");
                }
            }

            if (File.Exists(Config.SettingsPath))
            {
                try
                {
                    using var stream = new FileStream(Config.SettingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(stream);
                    var settingsContent = reader.ReadToEnd();

                    var tempSettings = JsonConvert.DeserializeObject<AppSettings>(settingsContent);

                    if (tempSettings != null)
                    {
                        Settings = tempSettings;
                    }

                    File.WriteAllText(Config.SettingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading settings file: {ex.Message}");
                }
            }
            else
            {
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

            _settingsWatcher.Changed += (sender, e) =>
            {
                LoadSettings(false);
            };

            _settingsWatcher.Error += (sender, e) =>
            {
                Custom.WriteLine($"FileSystemWatcher error: {e.GetException()?.Message}", ConsoleColor.Red);
            };
        }

        public static void StartProgram(int moduleIndex)
        {
            LoadSettings(true);

            Module = Config.ModuleBinding[moduleIndex];

            Console.Title = $"NetGuard | {Module.Name}";

            Config.LogFile = Path.Combine(Config.LogFolder, Module.Name + ".txt");

            Directory.CreateDirectory(Config.LogFolder);

            if (File.Exists(Config.LogFile))
                File.Delete(Config.LogFile);

            if (Module.ModuleType == ModuleType.AgentModule)
            {
                Reader.Init();
                PacketType.Init();

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

            // Use these values in your method as needed
            Custom.WriteLine($"Guard IP: {Module.GuardIP}, Guard Port: {Module.GuardPort}, Module IP: {Module.ModuleIP}, Module Port: {Module.ModulePort}", ConsoleColor.Magenta);

            Task.Run(() => StartAsyncServer());
            new Thread(ConsolePoolThread).Start();
        }

        static async Task StartAsyncServer()
        {
            AsyncServer server = new();
            await server.StartAsync(Module.GuardIP, Module.GuardPort, Module.ModuleType);
        }
    }
}
