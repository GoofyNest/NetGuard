using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public class ConfigUpdate : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            //byte action = packet.ReadUInt8();

            //if (action == 2) // Auto Potion
            //{
            //    // Something with HP
            //    byte hp_unk = packet.ReadUInt8();
            //    byte hp_unk2 = packet.ReadUInt8();
            //
            //    // Something with MP
            //    byte mp_unk = packet.ReadUInt8();
            //    byte mp_unk2 = packet.ReadUInt8();
            //
            //    // Unknown right now
            //    byte unk5 = packet.ReadUInt8();
            //    byte unk6 = packet.ReadUInt8();
            //
            //    byte delay = packet.ReadUInt8();
            //
            //}

            return response;
        }
    }
}
