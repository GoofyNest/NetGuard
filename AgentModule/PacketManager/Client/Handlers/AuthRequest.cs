using System;
using AgentModule.Engine.Classes;
using AgentModule.Framework;
using SilkroadSecurityAPI;
using Module;
using static AgentModule.Framework.Opcodes;
using static AgentModule.Framework.Opcodes.Server;
using System.Linq;

namespace AgentModule.PacketManager.Client.Handlers
{
    public class AuthRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            UInt32 Token = packet.ReadUInt32(); //from LOGIN_RESPONSE
            client.StrUserID = packet.ReadAscii();
            client.password = packet.ReadAscii();
            byte OperationType = packet.ReadUInt8();

            byte[] mac = packet.ReadUInt8Array(6);
            string mac_address = BitConverter.ToString(mac);
            int fail_count = mac.Count(b => b == 0x00);

            client.mac = mac_address;

            return response;
        }
    }
}
