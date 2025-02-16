using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Server.Packets
{
    public class PatchResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            client.Gateway.SentPatchResponse = true;

            return response;
        }
    }
}
