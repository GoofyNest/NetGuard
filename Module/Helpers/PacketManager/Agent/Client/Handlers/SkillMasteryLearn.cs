using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class SkillMasteryLearn : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            packet.ReadUInt32(); // masteryid

            var level = packet.ReadUInt8();

            if(level > 1)
                response.ResultType = PacketResultType.Block;

            return response;
        }
    }
}
