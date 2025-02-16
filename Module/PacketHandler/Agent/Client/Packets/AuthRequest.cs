using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public class AuthRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            uint Token = packet.ReadUInt32(); //from LOGIN_RESPONSE
            client.PlayerInfo.AccInfo = new()
            {
                Username = packet.ReadAscii(),
                Password = packet.ReadAscii()
            };
            byte OperationType = packet.ReadUInt8();

            byte[] mac = packet.ReadUInt8Array(6);
            string mac_address = BitConverter.ToString(mac);
            int fail_count = mac.Count(b => b == 0x00);

            client.PlayerInfo.AccInfo.Mac = mac_address;

            return response;
        }
    }
}
