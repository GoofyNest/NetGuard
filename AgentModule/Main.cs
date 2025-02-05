using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        static void ConsolePoolThread()
        {
            while (true)
            {
                Thread.Sleep(1);

                string cmd = Console.ReadLine();
            }
        }

        public async void StartProgram(string discordId, string discordName, string date)
        {
            
            //_appDomain.SetData("Assembly", null);

            var path = "C:\\Users\\computer\\source\\repos\\NetGuard\\scripting\\agent";

            var clientPath = Path.Combine(path, "client");
            var serverPath = Path.Combine(path, "server");

            Console.Title = "NetGuard | AgentModule";

            _clientPlugins.Add(new Plugin
            {
                ID = 0,
                name = "SamplePlugin",
                sourceCode = File.ReadAllText(Path.Combine(clientPath, "inCharSelection.csx")),
                filePath = Path.Combine(clientPath, "inCharSelection.csx")
            });

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

            await startAgent();

            new Thread(ConsolePoolThread).Start();
        }

        async Task startAgent()
        {
            AsyncServer _server = new AsyncServer();
            await _server.StartAsync("100.127.205.174", 15884, AsyncServer.E_ServerType.AgentModule);
        }
    }
}
