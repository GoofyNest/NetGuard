using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server.Packets
{
    public class CharacterSelectionResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            client.Agent.InCharSelectionScreen = true;

            return response;
        }
    }
}
