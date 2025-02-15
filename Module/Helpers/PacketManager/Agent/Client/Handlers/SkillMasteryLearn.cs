using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
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
            {
                Custom.WriteLine($"Prevented {client.playerInfo.accInfo.username}, using MasteryLevel exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }
                

            return response;
        }
    }
}
