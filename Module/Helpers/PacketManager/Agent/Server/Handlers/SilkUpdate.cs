using Module.Engine.Classes;
using Module.Framework;
using Module.Helpers.PacketManager.Agent.Client;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Server.Handlers
{
    public class SilkUpdate : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            var _settings = Main._settings;

            var adminIndex = _settings.Agent.GameMasters.FindIndex(m => m.Username == client.PlayerInfo.AccInfo.Username);
            if (adminIndex > -1)
            {
                var _admin = _settings.Agent.GameMasters[adminIndex];

                if (_admin.ShouldSpawnVisible)
                {
                    response.ModifiedPackets.Add(new PacketList() { Packet = packet });
                    var modified = new Packet((ushort)Operator.Command);
                    modified.WriteUInt8(14);
                    response.ModifiedPackets.Add(new PacketList() { Packet = modified, securityType = SecurityType.RemoteSecurity, SendImmediately = false });
                }
            }

            return response;
        }
    }
}
