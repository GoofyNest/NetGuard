using Module.Networking;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Client.Packets
{
    public class LoginRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            byte locale = packet.ReadUInt8();

            Custom.WriteLine($"locale: {locale}", ConsoleColor.Magenta);

            client.PlayerInfo.AccInfo = new();

            var accInfo = client.PlayerInfo.AccInfo;

            accInfo.Username = packet.ReadAscii();
            accInfo.Password = packet.ReadAscii();

            client.Gateway.ServerID = packet.ReadUInt16();

            if (!client.Gateway.SentPatchResponse || !client.Gateway.SentShardListResponse)
            {
                Custom.WriteLine($"sentPatchResponse: {client.Gateway.SentPatchResponse}", ConsoleColor.Magenta);
                Custom.WriteLine($"sentShardListResponse: {client.Gateway.SentShardListResponse}", ConsoleColor.Magenta);

                Custom.WriteLine($"Blocked potential exploit from {accInfo.Username} {accInfo.Password} {client.PlayerInfo.IpInfo.Ip} for exploiting", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
