using Module.Engine.Classes;
using Module.Framework;
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
                response.ResultType = PacketResultType.Block;

            return response;
        }
    }
}
