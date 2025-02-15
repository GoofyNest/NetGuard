using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
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
