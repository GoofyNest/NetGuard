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

            response.ModifiedPacket = null!;

            var modifiedPacket = new Packet((ushort)Game.Ready1);
            response.ModifiedPacket = modifiedPacket;

            response.SendImmediately = false;

            return response;
        }
    }
}
