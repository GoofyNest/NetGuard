using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Server.Handlers
{
    public class SilkUpdate : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var Settings = Main.Settings;

            var adminIndex = Settings.Agent.GameMasters.FindIndex(m => m.Username == client.PlayerInfo.AccInfo.Username);
            if (adminIndex > -1)
            {
                var _admin = Settings.Agent.GameMasters[adminIndex];

                if (_admin.ShouldSpawnVisible)
                {
                    response.ModifiedPackets.Add(new PacketList() { Packet = packet });

                    var test = new Packet(0x7010);
                    test.WriteUInt8(14);

                    response.ModifiedPackets.Add(new PacketList() { Packet = test, SecurityType = SecurityType.RemoteSecurity, SendImmediately = true });
                }
            }

            return response;
        }
    }
}
