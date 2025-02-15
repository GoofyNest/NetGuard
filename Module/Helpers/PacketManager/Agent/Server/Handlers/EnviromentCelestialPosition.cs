using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Server.Handlers
{
    public class EnviromentCelestialPosition : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            uint uniqueID = packet.ReadUInt32();
            ushort moonPhase = packet.ReadUInt16();
            byte hour = packet.ReadUInt8();
            byte minute = packet.ReadUInt8();

            return response;
        }
    }
}
