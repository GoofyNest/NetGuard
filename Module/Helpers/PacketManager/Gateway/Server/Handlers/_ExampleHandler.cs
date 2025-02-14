﻿using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Gateway.Server.Handlers
{
    public class _ExampleHandler : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            return response;
        }
    }
}
