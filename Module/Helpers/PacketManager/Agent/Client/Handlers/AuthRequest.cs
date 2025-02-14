using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class AuthRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            uint Token = packet.ReadUInt32(); //from LOGIN_RESPONSE
            client.playerInfo.accInfo = new();
            client.playerInfo.accInfo.username = packet.ReadAscii();
            client.playerInfo.accInfo.password = packet.ReadAscii();
            byte OperationType = packet.ReadUInt8();

            byte[] mac = packet.ReadUInt8Array(6);
            string mac_address = BitConverter.ToString(mac);
            int fail_count = mac.Count(b => b == 0x00);

            client.playerInfo.accInfo.mac = mac_address;

            return response;
        }
    }
}
