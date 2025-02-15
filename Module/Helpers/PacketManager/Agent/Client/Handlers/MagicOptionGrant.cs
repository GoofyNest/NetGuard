using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
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
            {
                Custom.WriteLine($"Prevented {client.playerInfo.accInfo.username}, using avatar packet on jSRO", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }
                

            var avatarBlue = packet.ReadAscii().ToLower();

            if (avatarBlue.Contains("avatar"))
            {
                Custom.WriteLine($"Prevented {client.playerInfo.accInfo.username}, using avatar blue exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
