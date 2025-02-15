using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class PlayerBerserk : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var flag = packet.ReadUInt8();

            if (flag > 1)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, using Invisible exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }
                

            return response;
        }
    }
}
