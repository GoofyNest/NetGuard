﻿using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class Handshake : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            response.SendImmediately = false;
            response.ResultType = PacketResultType.SkipSending;

            return response;
        }
    }
}
