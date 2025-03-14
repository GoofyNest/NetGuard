﻿using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server.Packets
{
    public class CharacterJoin : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            if (client.PlayerInfo.CharInfo == null)
                client.PlayerInfo.CharInfo = [];

            var charInfo = client.PlayerInfo.CharInfo;

            var index = charInfo.FindIndex(m => m.Charname == client.PlayerInfo.CurrentCharName);

            if (index == -1)
                charInfo.Add(new CharacterInformation() { Charname = client.PlayerInfo.CurrentCharName });

            client.Agent.SentJoinRequest = true;

            return response;
        }
    }
}
