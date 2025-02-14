using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class MagicOptionGrant : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            var _files = Main._settings.serverVersion.CurrentValue;

            if (_files == "jSRO")
                response.ResultType = PacketResultType.Block;

            var avatarBlue = packet.ReadAscii().ToLower();

            if (avatarBlue.Contains("avatar"))
                response.ResultType = PacketResultType.Block;

            return response;
        }
    }
}
