using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Gateway.Server.Handlers
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
