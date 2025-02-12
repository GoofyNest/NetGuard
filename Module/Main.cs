using Module.Classes;
using Module.Engine;
using Newtonsoft.Json;

namespace Module
{
    public class Main
    {
        public static Config _config = new Config();
        public static ModuleSettings _module = new ModuleSettings();
        public static Settings _settings = new Settings();

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
            if (File.Exists(_config.settingsPath))
            {
                try
                {
                    using (var stream = new FileStream(_config.settingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

            if (File.Exists(_config.bindingsPath))
            {
                try
                {
                    using (var stream = new FileStream(_config.bindingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream))
                    {
                        var bindingsContent = reader.ReadToEnd();

                        var tempConfig = JsonConvert.DeserializeObject<Config>(bindingsContent);

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
        }

        private static void WatchSettingsFile()
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(_config.settingsPath)!,
                Filter = Path.GetFileName(_config.settingsPath),
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
            

            Directory.CreateDirectory("config");

            LoadSettings(true);

            _module = _config.ModuleBinding[moduleIndex];

            Console.Title = $"NetGuard | {_module.name}";

            // Use these values in your method as needed
            Console.WriteLine($"Guard IP: {_module.guardIP}, Guard Port: {_module.guardPort}, Module IP: {_module.moduleIP}, Module Port: {_module.modulePort}");

            _config.logFile = Path.Combine(_config.logFolder, _module.name + ".txt");

            Directory.CreateDirectory(_config.logFolder);

            if (File.Exists(_config.logFile))
                File.Delete(_config.logFile);

            WatchSettingsFile();

            Task.Run(() => startAsyncServer());

            new Thread(ConsolePoolThread).Start();
        }

        static async Task startAsyncServer()
        {
            AsyncServer server = new AsyncServer();
            await server.StartAsync(_module.guardIP, _module.guardPort, _module.moduleType);
        }
    }
}
