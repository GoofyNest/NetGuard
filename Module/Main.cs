using Module.Classes;
using Module.Engine;
using Module.Helpers.ItemReader;
using Module.Helpers.PacketManager.Agent.Client;
using Newtonsoft.Json;
using static Module.Classes.Settings;

namespace Module
{
    public class Main
    {
        public static Module.Classes.Config _config = new();
        public static ModuleSettings _module = new();
        public static Settings _settings = new();
        public static Dictionary<int, ItemData> _items = new();

        static void ConsolePoolThread()
        {
            while (true)
            {
                Thread.Sleep(1);

                string cmd = Console.ReadLine() ?? string.Empty;
            }
        }

        private static void LoadSettings(bool isMainFunction)
        {
            if (File.Exists(_config.SettingsPath))
            {
                try
                {
                    using (var stream = new FileStream(_config.SettingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream))
                    {
                        var settingsContent = reader.ReadToEnd();

                        var tempSettings = JsonConvert.DeserializeObject<Settings>(settingsContent);
                        if(tempSettings != null)
                        {
                            _settings = tempSettings;
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error reading settings file: {ex.Message}");
                }
            }

            if (!isMainFunction)
                return;

            if (File.Exists(_config.BindingsPath))
            {
                try
                {
                    using (var stream = new FileStream(_config.BindingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream))
                    {
                        var bindingsContent = reader.ReadToEnd();

                        var tempConfig = JsonConvert.DeserializeObject<Module.Classes.Config>(bindingsContent);

                        if (tempConfig != null)
                        {
                            _config = tempConfig;
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error reading settings file: {ex.Message}");
                }
            }

            if (File.Exists(_config.SettingsPath))
            {
                var settingsContent = File.ReadAllText(_config.SettingsPath);

                var tempSettings = JsonConvert.DeserializeObject<Settings>(settingsContent);
                if (tempSettings != null)
                {
                    _settings = tempSettings;
                }

                File.WriteAllText(_config.SettingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
            }
            else
            {
                File.WriteAllText(_config.SettingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
            }
        }

        private static void WatchSettingsFile()
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(_config.SettingsPath)!,
                Filter = Path.GetFileName(_config.SettingsPath),
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

            _module = _config.ModuleBinding[moduleIndex];

            Console.Title = $"NetGuard | {_module.Name}";

            // Use these values in your method as needed
            Console.WriteLine($"Guard IP: {_module.GuardIP}, Guard Port: {_module.GuardPort}, Module IP: {_module.ModuleIP}, Module Port: {_module.ModulePort}");

            _config.LogFile = Path.Combine(_config.LogFolder, _module.Name + ".txt");

            Directory.CreateDirectory(_config.LogFolder);

            if (File.Exists(_config.LogFile))
                File.Delete(_config.LogFile);

            if (_module.ModuleType == ModuleType.AgentModule)
            {
                ItemReader.Init();
                Packets.Init();

                if (_settings.Agent.GameMasters.Count() == 0)
                {
                    var example = new Operators();
                    example.Username = "netguard_example_325125124123124";
                    example.NoPinkChat = false;
                    example.Permissions = example.DefaultPermissions;
                    example.ShouldSpawnVisible = false;

                    _settings.Agent.GameMasters.Add(example);

                    File.WriteAllText(_config.SettingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
                }
            }

            WatchSettingsFile();

            Task.Run(() => startAsyncServer());

            new Thread(ConsolePoolThread).Start();
        }

        static async Task startAsyncServer()
        {
            AsyncServer server = new();
            await server.StartAsync(_module.GuardIP, _module.GuardPort, _module.ModuleType);
        }
    }
}
