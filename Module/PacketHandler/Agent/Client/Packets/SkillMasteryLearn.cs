using Module.Networking;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public class SkillMasteryLearn : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            packet.ReadUInt32(); // masteryid

            var level = packet.ReadUInt8();

            if (level > 1)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, using MasteryLevel exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }


            return response;
        }
    }
}
