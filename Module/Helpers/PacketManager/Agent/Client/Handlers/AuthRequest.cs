using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using _Agent = Module.Helpers.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.Helpers.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.Helpers.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.Helpers.PacketManager.Agent.ClientPackets.Shard;

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
