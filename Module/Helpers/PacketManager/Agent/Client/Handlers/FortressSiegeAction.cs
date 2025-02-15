﻿using System.Text.RegularExpressions;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class FortressSiegeAction : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var message = packet.ReadAscii();

            if (Regex.IsMatch(message, @"['""\-]"))
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, attempted SQL injection", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
