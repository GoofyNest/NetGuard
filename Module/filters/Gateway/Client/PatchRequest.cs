using Module.Networking;
using Module.PacketHandler;
using Module.PacketHandler.Filtering;
using Module.Services;
using SilkroadSecurityAPI;

namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x6100)]
    public sealed class PatchRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

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