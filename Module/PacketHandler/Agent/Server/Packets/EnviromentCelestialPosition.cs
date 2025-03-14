﻿using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server.Packets
{
    public class EnviromentCelestialPosition : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            //uint uniqueID = packet.ReadUInt32();
            //ushort moonPhase = packet.ReadUInt16();
            //byte hour = packet.ReadUInt8();
            //byte minute = packet.ReadUInt8();

            var charInfo = client.PlayerInfo.CharInfo.Find(m => m.Charname == client.PlayerInfo.CurrentCharName);

            if (charInfo == null)
                return response;

            charInfo.IsVisible = false;

            return response;
        }
    }
}
