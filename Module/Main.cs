using Module.Classes;
using Module.Engine;
using Newtonsoft.Json;

namespace Module
{
    public class Main
    {
        public static DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        public static long unixTimestamp = dateTimeOffset.ToUnixTimeSeconds();

        public static Config _config = new Config();
        public static ModuleSettings _module = new ModuleSettings();
        public static Settings _settings = new Settings();

        public static string logFile = "";

        static void ConsolePoolThread()
        {
            while (true)
            {
                Thread.Sleep(1);

                string cmd = Console.ReadLine() ?? string.Empty;
            }
        }

        public static void StartProgram(int moduleIndex)
        {
            Console.Title = "NetGuard | .NET 8.0";

            var path = Path.Combine("config");

            Directory.CreateDirectory(path);

            var bindingsPath = Path.Combine(path, "bindings.json");
            var settingsPath = Path.Combine(path, "settings.json");

            if (File.Exists(bindingsPath))
            {
                var bindingsContent = File.ReadAllText(bindingsPath);

                _config = JsonConvert.DeserializeObject<Config>(bindingsContent)
                          ?? throw new InvalidOperationException("bindings JSON is null.");
            }

            if (File.Exists(settingsPath))
            {
                var settingsContent = File.ReadAllText(settingsPath);

                _config = JsonConvert.DeserializeObject<Config>(settingsContent)
                          ?? throw new InvalidOperationException("settings JSON is null.");

                File.WriteAllText(settingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
            }
            else
            {
                File.WriteAllText(settingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
            }

            _module = _config.ModuleBinding[moduleIndex];

            // Use these values in your method as needed
            Console.WriteLine($"Guard IP: {_module.guardIP}, Guard Port: {_module.guardPort}, Module IP: {_module.moduleIP}, Module Port: {_module.modulePort}");

            logFile = Path.Combine(path, _module.name + ".txt");

            if (File.Exists(logFile))
                File.Delete(logFile);

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
