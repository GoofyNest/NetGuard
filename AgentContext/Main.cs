using NetGuard.Engine;
using System;
using System.Threading.Tasks;

namespace AgentContext
{
    public class Main
    {
        public void StartProgram(string discordId, string discordName, string date)
        {
            Console.Title = "NetGuard | AgentContext";

            startGateway();
        }

        public async void startGateway()
        {
            AsyncServer _server = new AsyncServer();

            _server.Start("100.127.205.174", 15884, AsyncServer.E_ServerType.AgentServer);

            await Task.Delay(1);
        }
    }
}
