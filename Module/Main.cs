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

        public static ModuleSettings _moduleSettings = null!;

        public static string logFile = "";

        public static AsyncServer.E_ServerType _handlerType;

        static void ConsolePoolThread()
        {
            while (true)
            {
                Thread.Sleep(1);

                string cmd = Console.ReadLine() ?? string.Empty;
            }
        }

        public static void StartProgram(string module, string name, string guardIP, int guardPort, string moduleIP, int modulePort)
        {
            Console.Title = "NetGuard | TestModule";

            // Use these values in your method as needed
            Console.WriteLine($"Guard IP: {guardIP}, Guard Port: {guardPort}, Module IP: {moduleIP}, Module Port: {modulePort}");

            _moduleSettings = new ModuleSettings { guardIP = guardIP, guardPort = guardPort, moduleIP = moduleIP, modulePort = modulePort };

            var path = Path.Combine("config");

            Directory.CreateDirectory(path);

            var settingsPath = Path.Combine(path, "settings.json");

            if (File.Exists(settingsPath))
            {
                var configContent = File.ReadAllText(settingsPath);

                _config = JsonConvert.DeserializeObject<Config>(configContent)
                          ?? throw new InvalidOperationException("Configuration JSON is null.");
            }

            logFile = Path.Combine(path, name + ".txt");

            if (File.Exists(logFile))
                File.Delete(logFile);

            if (module == "gateway")
            {
                _handlerType = AsyncServer.E_ServerType.GatewayModule;
            }
            else if (module == "agent")
            {
                _handlerType = AsyncServer.E_ServerType.AgentModule;
            }

            Task.Run(() => startAsyncServer());

            new Thread(ConsolePoolThread).Start();
        }

        static async Task startAsyncServer()
        {
            AsyncServer server = new AsyncServer();
            await server.StartAsync(_moduleSettings.guardIP, _moduleSettings.guardPort, _handlerType);
        }
    }
}
