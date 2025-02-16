using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public class SpoofLocaleVersion : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var modified = new Packet((ushort)Game.Ready1);

            response.ModifiedPackets.Add(new PacketList { Packet = modified });

            return response;
        }
    }
}
