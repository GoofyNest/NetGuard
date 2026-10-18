namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA102)] // SERVER_GATEWAY_LOGIN_RESPONSE
    public sealed class LoginResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            byte res = packet.ReadUInt8();

            Custom.WriteLine($"SERVER_GATEWAY_LOGIN_RESPONSE {res}");

            if (res == 1)
            {
                uint id     = packet.ReadUInt32();
                string host = packet.ReadAscii();
                int port    = packet.ReadUInt16();

                var index = Main.Config.ModuleBinding.FindIndex(m => m.ModuleIP == host && m.ModulePort == port);
                if (index == -1)
                {
                    Custom.WriteLine("Could not find agent bindings", ConsoleColor.Red);
                    return response;
                }

                client.Gateway.SentLoginRequest = true;
                foreach (var agent in Main.Config.ModuleBinding.Where(m => m.ModuleType == ModuleType.AgentModule))
                {
                    SessionBus.SendTo(
                        agent.ApiHost,
                        agent.ApiPort,
                        agent.ApiPassword,
                        new BusMessage("gateway.login_requested", new
                        {
                            Username = client.PlayerInfo.Username ?? "",
                            Target   = $"{host}:{port}",
                        }));
                }

                var guardModule = Main.Config.ModuleBinding[index];

                Custom.WriteLine($"Using {guardModule.GuardIP} {guardModule.GuardPort}", ConsoleColor.Cyan);

                var modified = new Packet(0xA102, true); // SERVER_GATEWAY_LOGIN_RESPONSE
                modified.WriteUInt8(res);
                modified.WriteUInt32(id);
                modified.WriteAscii(guardModule.GuardIP);
                modified.WriteUInt16((ushort)guardModule.GuardPort);
                modified.WriteUInt32(0);

                response.ModifiedPackets.Add(new PacketList { Packet = modified });
                return response;
            }

            return response;
        }
    }
}