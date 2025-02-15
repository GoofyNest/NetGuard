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

            response.ModifiedPacket = null!;

            var flag = packet.ReadUInt8();

            if (flag > 1)
            {
                Custom.WriteLine($"Prevented {client.playerInfo.accInfo.username}, using Invisible exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }
                

            return response;
        }
    }
}
