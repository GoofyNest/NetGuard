using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Server.Packets
{
    public class ShardListResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            client.Gateway.SentShardListResponse = true;

            return response;
        }
    }
}
