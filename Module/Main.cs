using Module.Classes;
using Module.Engine;
using Module.Helpers;
using Module.Helpers.ClientData;
using Module.Helpers.PacketManager.Agent.Client;
using Module.Services;
using Newtonsoft.Json;

namespace Module
{
    public class Main
    {
        public static Configuration Config { get; set; } = new();
        public static ModuleSettings Module { get; set; } = new();
        public static ProgramSettings Settings { get; set; } = new();
        public static Dictionary<int, ItemData> Items { get; set; } = [];
        public static Dictionary<int, SkillData> Skills { get; set; } = [];

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
            if (File.Exists(Config.SettingsPath))
            {
                try
                {
                    using var stream = new FileStream(Config.SettingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(stream);
                    var settingsContent = reader.ReadToEnd();

                    var tempSettings = JsonConvert.DeserializeObject<ProgramSettings>(settingsContent);
                    if (tempSettings != null)
                    {
                        Settings = tempSettings;
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error reading settings file: {ex.Message}");
                }
            }

            if (!isMainFunction)
                return;

            if (File.Exists(Config.BindingsPath))
            {
                try
                {
                    using var stream = new FileStream(Config.BindingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(stream);
                    var bindingsContent = reader.ReadToEnd();

                    var tempConfig = JsonConvert.DeserializeObject<Configuration>(bindingsContent);

                    if (tempConfig != null)
                    {
                        Config = tempConfig;
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error reading settings file: {ex.Message}");
                }
            }

            if (File.Exists(Config.SettingsPath))
            {
                var settingsContent = File.ReadAllText(Config.SettingsPath);

                var tempSettings = JsonConvert.DeserializeObject<ProgramSettings>(settingsContent);
                if (tempSettings != null)
                {
                    Settings = tempSettings;
                }

                File.WriteAllText(Config.SettingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
            else
            {
                File.WriteAllText(Config.SettingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
        }

        private static void WatchSettingsFile()
        {
            FileSystemWatcher watcher = new()
            {
                Path = Path.GetDirectoryName(Config.SettingsPath)!,
                Filter = Path.GetFileName(Config.SettingsPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                EnableRaisingEvents = true
            };

            watcher.Changed += (sender, e) =>
            {
                LoadSettings(false);
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
                Packets.Init();

                if (Settings.Agent.GameMasters.Count == 0)
                {
                    var example = new ProgramSettings.Operators
                    {
                        Username = "netguard_example_325125124123124",
                        NoPinkChat = false,
                        ShouldSpawnVisible = false
                    };
                    example.Permissions = example.DefaultPermissions;

                    Settings.Agent.GameMasters.Add(example);

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
