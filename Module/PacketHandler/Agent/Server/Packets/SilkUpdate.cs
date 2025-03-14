﻿using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server.Packets
{
    public class SilkUpdate : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var Settings = Main.Settings;
            var charInfo = client.PlayerInfo.CharInfo.Find(m => m.Charname == client.PlayerInfo.CurrentCharName);

            if (charInfo == null)
                return response;

            var adminIndex = Settings.Agent.GameMasterConfig.GMs.FindIndex(m => m.Username == client.PlayerInfo.AccInfo.Username);

            if (adminIndex == -1 && Settings.Agent.GameMasterConfig.Misc.SpawnVisible)
            {
                response.ModifiedPackets.Add(new PacketList() { Packet = packet });

                var test = new Packet(0x7010);
                test.WriteUInt8(14);

                response.ModifiedPackets.Add(new PacketList() { Packet = test, SecurityType = SecurityType.RemoteSecurity, SendImmediately = true });
                charInfo.IsVisible = true;
            }
            else if(adminIndex > -1)
            {
                var _admin = Settings.Agent.GameMasterConfig.GMs[adminIndex];

                if (_admin.ShouldSpawnVisible && !charInfo.IsVisible)
                {
                    response.ModifiedPackets.Add(new PacketList() { Packet = packet });

                    var test = new Packet(0x7010);
                    test.WriteUInt8(14);

                    response.ModifiedPackets.Add(new PacketList() { Packet = test, SecurityType = SecurityType.RemoteSecurity, SendImmediately = true });
                    charInfo.IsVisible = true;
                }
            }

            return response;
        }
    }
}
