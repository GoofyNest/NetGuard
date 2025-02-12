using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Client.Handlers
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
