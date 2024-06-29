using NetGuard.Engine;
using System;
using System.Threading.Tasks;

namespace GatewayContext
{
    public class Main
    {
        public void StartProgram(string discordId, string discordName, string date)
        {
            Console.Title = "NetGuard | GatewayContext";

            startGateway();
        }

        public async void startGateway()
        {
            AsyncServer server = new AsyncServer();
            await server.StartAsync("100.127.205.174", 15779, AsyncServer.E_ServerType.GatewayServer);

            await Task.Delay(1);
        }
    }
}
