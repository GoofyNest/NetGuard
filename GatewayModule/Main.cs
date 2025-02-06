﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GatewayModule.Classes;
using NetGuard.Classes;
using NetGuard.Engine;
using Newtonsoft.Json;

namespace Module
{
    public class Main
    {
        public static Config _config = new Config();

        public static DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        public static long unixTimestamp = dateTimeOffset.ToUnixTimeSeconds();

        public static ModuleSettings _moduleSettings = null;

        static void ConsolePoolThread()
        {
            while (true)
            {
                Thread.Sleep(1);

                string cmd = Console.ReadLine();
            }
        }

        public void StartProgram(string guardIP, int guardPort, string moduleIP, int modulePort)
        {
            Console.Title = "NetGuard | GatewayModule";

            // Use these values in your method as needed
            Console.WriteLine($"Guard IP: {guardIP}, Guard Port: {guardPort}, Module IP: {moduleIP}, Module Port: {modulePort}");

            _moduleSettings = new ModuleSettings { guardIP = guardIP, guardPort = guardPort, moduleIP = moduleIP, modulePort = modulePort };

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
            await server.StartAsync(_moduleSettings.guardIP, _moduleSettings.guardPort, AsyncServer.E_ServerType.GatewayModule);
        }
    }
}
