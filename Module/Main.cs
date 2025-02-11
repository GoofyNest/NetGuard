using System.Reflection;
using Module.Classes;
using Module.Engine;
using Module.Services;
using Newtonsoft.Json;

namespace Module
{
    public class Main
    {
        public static Config _config = new Config();

        public static DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        public static long unixTimestamp = dateTimeOffset.ToUnixTimeSeconds();

        public static ModuleSettings _module = null!;

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

            var settingsPath = Path.Combine(path, "settings.json");

            if (File.Exists(settingsPath))
            {
                var configContent = File.ReadAllText(settingsPath);

                _config = JsonConvert.DeserializeObject<Config>(configContent)
                          ?? throw new InvalidOperationException("Configuration JSON is null.");
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
