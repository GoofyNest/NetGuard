using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using _Agent = Module.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.PacketManager.Agent.ClientPackets.Shard;

namespace Module.PacketManager.Agent.Client.Handlers
{
    public class AuthRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

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
