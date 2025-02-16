using Module.Networking;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Client.Packets
{
    public class NoticeRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            //byte contentID = packet.ReadUInt8();

            if (packet.GetBytes().Length > 1)
                response.ResultType = PacketResultType.Block;

            Custom.WriteLine($"CLIENT_GATEWAY_NOTICE_REQUEST {packet.GetBytes().Length}", ConsoleColor.Magenta);

            return response;
        }
    }
}
