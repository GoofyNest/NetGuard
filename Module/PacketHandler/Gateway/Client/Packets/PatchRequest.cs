using Module.Networking;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Client.Packets
{
    public class PatchRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            byte contentID = packet.ReadUInt8();
            string ModuleName = packet.ReadAscii();
            uint version = packet.ReadUInt32();

            Custom.WriteLine($"contentID {contentID}");
            Custom.WriteLine($"ModuleName {ModuleName}");
            Custom.WriteLine($"version {version}");

            return response;
        }
    }
}
