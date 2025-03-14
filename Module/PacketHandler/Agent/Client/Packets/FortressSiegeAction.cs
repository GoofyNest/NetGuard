﻿using System.Text.RegularExpressions;
using Module.Networking;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public partial class FortressSiegeAction : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var message = packet.ReadAscii();

            if (SQLInjectPrevention().IsMatch(message))
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, attempted SQL injection", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }

        // Removing the 'static' modifier, since it's generated automatically
        [GeneratedRegex(@"['""\-]")]
        private partial Regex SQLInjectPrevention();
    }
}