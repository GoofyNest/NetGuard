using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3153)]
    public sealed class SilkUpdate : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            var Settings = Main.Settings;
            var charInfo = client.CurrentChar();
            if (charInfo == null) return response;

            var adminIndex = Settings.Agent.GameMasterConfig.GMs
                .FindIndex(m => m.Username == client.PlayerInfo.Username);

            if (adminIndex == -1 && Settings.Agent.GameMasterConfig.Misc.SpawnVisible)
            {
                response.ModifiedPackets.Add(new PacketList() { Packet = packet });
                var test = new Packet(0x7010);
                test.WriteUInt8(14);
                response.ModifiedPackets.Add(new PacketList() { Packet = test, SecurityType = SecurityType.RemoteSecurity, SendImmediately = true });
                charInfo.IsVisible = true;
            }
            else if (adminIndex > -1)
            {
                var _admin = Settings.Agent.GameMasterConfig.GMs[adminIndex];
                if (_admin.ShouldSpawnVisible && !charInfo.IsVisible)
                {
                    response.ModifiedPackets.Add(new PacketList() { Packet = packet });
                    var test = new Packet(0x7010);
                    test.WriteUInt8(14);
                    response.ModifiedPackets.Add(new PacketList() { Packet = test, SecurityType = SecurityType.RemoteSecurity, SendImmediately = true });
                    charInfo.IsVisible = true;
                }
            }

            return response;
        }
    }
}