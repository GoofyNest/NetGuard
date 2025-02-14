using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Client;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Server;

namespace Module.Helpers.PacketManager.Gateway.Server.Handlers
{
    public class Handshake : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            response.SendImmediately = true;
            response.ResultType = PacketResultType.SkipSending;

            return response;
        }
    }
}
