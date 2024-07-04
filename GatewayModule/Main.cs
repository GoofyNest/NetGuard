using NetGuard.Classes;
using NetGuard.Engine;
using NetGuard.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Module
{
    public class Main
    {
        public static Config _config = new Config();

        public static DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        public static long unixTimestamp = dateTimeOffset.ToUnixTimeSeconds();

        static void ConsolePoolThread()
        {
            while (true)
            {
                Thread.Sleep(1);

                string cmd = Console.ReadLine();
            }
        }

        public void StartProgram(string discordId, string discordName, string date)
        {
            Console.Title = "NetGuard | GatewayModule";

            var path = Path.Combine("config");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var settingsPath = Path.Combine(path, "settings.json");

            if (!File.Exists(settingsPath))
            {
                File.WriteAllText(settingsPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
            }
            else
            {
                _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(settingsPath));
            }

            startGateway();

            new Thread(ConsolePoolThread).Start();
        }

        public async void startGateway()
        {
            AsyncServer server = new AsyncServer();
            server.StartAsync("100.127.205.174", 15779, AsyncServer.E_ServerType.GatewayModule);
        }
    }
}
