using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AgentModule.Engine.Classes;
using NetGuard.Engine;

namespace Module
{
    public class Main
    {
        public static AppDomain _appDomain = AppDomain.CurrentDomain;

        public static DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        public static long unixTimestamp = dateTimeOffset.ToUnixTimeSeconds();

        public static List<Plugin> _clientPlugins = new List<Plugin>();
        public static List<Plugin> _serverPlugins = new List<Plugin>();

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
            Console.Title = "NetGuard | AgentModule";

            // Use these values in your method as needed
            Console.WriteLine($"Guard IP: {guardIP}, Guard Port: {guardPort}, Module IP: {moduleIP}, Module Port: {modulePort}");

            _moduleSettings = new ModuleSettings { guardIP = guardIP, guardPort = guardPort, moduleIP = moduleIP, modulePort = modulePort };

            //_clientPlugins.Add(new Plugin
            //{
            //    ID = 0,
            //    name = "SamplePlugin",
            //    sourceCode = File.ReadAllText(Path.Combine(clientPath, "inCharSelection.csx")),
            //    filePath = Path.Combine(clientPath, "inCharSelection.csx")
            //});

            //_clientPlugins.Add(0x3333, new Dictionary<string, object> {
            //    { "name", "SamplePlugin" },
            //    { "source", File.ReadAllText(Path.Combine(clientPath, "inCharSelection.csx")) }
            //});

            //pluginCode = File.ReadAllText(Path.Combine(clientPath, "inCharSelection.csx"));

            //_clientPlugins.Add()

            //_clientScripts = new Dictionary<ushort, string> {
            //    { 0x1111, @File.ReadAllText(Path.Combine(clientPath, "inCharSelection.csx")) }
            //    // Add more opcodes and their corresponding script paths here
            //};
            //
            //foreach(var script in _clientScripts.Keys)
            //{ 
            //    Console.WriteLine(script.ToString());
            //}

            startAgent();

            new Thread(ConsolePoolThread).Start();
        }

        async void startAgent()
        {
            AsyncServer _server = new AsyncServer();
            await _server.StartAsync(_moduleSettings.guardIP, _moduleSettings.guardPort, AsyncServer.E_ServerType.AgentModule);
        }
    }
}
