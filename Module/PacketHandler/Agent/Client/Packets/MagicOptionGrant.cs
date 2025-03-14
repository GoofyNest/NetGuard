﻿using Module.Networking;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public class MagicOptionGrant : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var _files = Main.Settings.ServerType.CurrentValue;

            if (_files == "jSRO")
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, using avatar packet on jSRO", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }


            var avatarBlue = packet.ReadAscii().ToLower();

            if (avatarBlue.Contains("avatar"))
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, using avatar blue exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
