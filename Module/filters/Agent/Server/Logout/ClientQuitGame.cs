namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x300A)]
    public sealed class ClientQuitGame : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();
            client.Agent.ClientQuitGame = true;

            foreach (var gw in Main.Config.ModuleBinding.Where(m => m.ModuleType == ModuleType.GatewayModule))
            {
                SessionBus.SendTo(
                    gw.ApiHost,
                    gw.ApiPort,
                    gw.ApiPassword,
                    new BusMessage("agent.quit_acknowledged", new { Username = client.PlayerInfo.Username }));
            }

            return response;
        }
    }
}